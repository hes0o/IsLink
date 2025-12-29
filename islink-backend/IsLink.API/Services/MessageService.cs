using IsLink.API.Data;
using IsLink.API.Models.DTOs;
using IsLink.API.Models.MongoDB;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace IsLink.API.Services;

public class MessageService : IMessageService
{
    private readonly IMongoCollection<Conversation> _conversations;
    private readonly IMongoCollection<Message> _messages;
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public MessageService(IMongoDatabase database, ApplicationDbContext context, INotificationService notificationService)
    {
        _conversations = database.GetCollection<Conversation>("conversations");
        _messages = database.GetCollection<Message>("messages");
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<ConversationListResponse> GetConversationsAsync(string userId)
    {
        var conversations = await _conversations
            .Find(c => c.Participants.Contains(userId) && c.IsActive)
            .SortByDescending(c => c.UpdatedAt)
            .ToListAsync();

        var conversationDtos = new List<ConversationDto>();

        foreach (var conv in conversations)
        {
            var otherParticipantId = conv.Participants.FirstOrDefault(p => p != userId);
            if (string.IsNullOrEmpty(otherParticipantId)) continue;

            if (!Guid.TryParse(otherParticipantId, out var participantGuid)) continue;

            var participant = await _context.Users
                .Where(u => u.Id == participantGuid)
                .Select(u => new { u.Username, u.FullName, u.AvatarUrl, u.IsOnline })
                .FirstOrDefaultAsync();

            conversationDtos.Add(new ConversationDto
            {
                Id = conv.Id,
                Participant = new ConversationParticipantDto
                {
                    Id = otherParticipantId,
                    Username = participant?.Username ?? "",
                    FullName = participant?.FullName ?? "",
                    Avatar = participant?.AvatarUrl,
                    IsOnline = participant?.IsOnline ?? false
                },
                LastMessage = conv.LastMessage == null ? null : new LastMessageDto
                {
                    Content = conv.LastMessage.Content,
                    SenderId = conv.LastMessage.SenderId,
                    Timestamp = conv.LastMessage.Timestamp
                },
                UnreadCount = conv.UnreadCount.TryGetValue(userId, out var count) ? count : 0,
                UpdatedAt = conv.UpdatedAt
            });
        }

        return new ConversationListResponse
        {
            Success = true,
            Data = conversationDtos
        };
    }

    public async Task<(bool Success, ConversationDto? Data)> GetOrCreateConversationAsync(string userId, CreateConversationRequest request)
    {
        if (userId == request.ParticipantId)
        {
            return (false, null);
        }

        // Check if participant exists
        if (!Guid.TryParse(request.ParticipantId, out var participantGuid))
        {
            return (false, null);
        }

        var participant = await _context.Users
            .Where(u => u.Id == participantGuid)
            .Select(u => new { u.Id, u.Username, u.FullName, u.AvatarUrl })
            .FirstOrDefaultAsync();

        if (participant == null)
        {
            return (false, null);
        }

        // Find existing conversation
        var existingConv = await _conversations
            .Find(c => c.Participants.Contains(userId) && c.Participants.Contains(request.ParticipantId))
            .FirstOrDefaultAsync();

        if (existingConv != null)
        {
            return (true, new ConversationDto
            {
                Id = existingConv.Id,
                Participant = new ConversationParticipantDto
                {
                    Id = participant.Id.ToString(),
                    Username = participant.Username,
                    FullName = participant.FullName,
                    Avatar = participant.AvatarUrl
                }
            });
        }

        // Create new conversation
        var conversation = new Conversation
        {
            Participants = new List<string> { userId, request.ParticipantId },
            RelatedGig = request.RelatedGig,
            RelatedOrder = request.RelatedOrder,
            UnreadCount = new Dictionary<string, int>
            {
                { userId, 0 },
                { request.ParticipantId, 0 }
            }
        };

        await _conversations.InsertOneAsync(conversation);

        return (true, new ConversationDto
        {
            Id = conversation.Id,
            Participant = new ConversationParticipantDto
            {
                Id = participant.Id.ToString(),
                Username = participant.Username,
                FullName = participant.FullName,
                Avatar = participant.AvatarUrl
            }
        });
    }

    public async Task<MessageListResponse> GetMessagesAsync(string userId, string conversationId, int limit, DateTime? before)
    {
        var conversation = await _conversations.Find(c => c.Id == conversationId).FirstOrDefaultAsync();
        if (conversation == null || !conversation.Participants.Contains(userId))
        {
            return new MessageListResponse { Success = false };
        }

        var filter = Builders<Message>.Filter.Eq(m => m.ConversationId, conversationId);
        if (before.HasValue)
        {
            filter &= Builders<Message>.Filter.Lt(m => m.CreatedAt, before.Value);
        }

        var messages = await _messages
            .Find(filter)
            .SortByDescending(m => m.CreatedAt)
            .Limit(limit)
            .ToListAsync();

        // Mark as read
        var update = Builders<Message>.Update
            .Set(m => m.IsRead, true)
            .Set(m => m.ReadAt, DateTime.UtcNow);

        await _messages.UpdateManyAsync(
            m => m.ConversationId == conversationId && m.SenderId != userId && !m.IsRead,
            update);

        // Reset unread count
        if (conversation.UnreadCount.ContainsKey(userId))
        {
            conversation.UnreadCount[userId] = 0;
            await _conversations.ReplaceOneAsync(c => c.Id == conversationId, conversation);
        }

        messages.Reverse();

        return new MessageListResponse
        {
            Success = true,
            Data = messages.Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                Content = m.Content,
                MessageType = m.MessageType,
                Attachments = m.Attachments.Select(a => new AttachmentDto
                {
                    FileName = a.FileName,
                    FileUrl = a.FileUrl,
                    FileSize = a.FileSize,
                    FileType = a.FileType
                }).ToList(),
                CustomOffer = m.CustomOffer == null ? null : new CustomOfferDto
                {
                    Price = m.CustomOffer.Price,
                    DeliveryDays = m.CustomOffer.DeliveryDays,
                    Description = m.CustomOffer.Description
                },
                IsRead = m.IsRead,
                CreatedAt = m.CreatedAt
            }).ToList()
        };
    }

    public async Task<(bool Success, MessageDto? Data)> SendMessageAsync(string userId, string conversationId, SendMessageRequest request)
    {
        var conversation = await _conversations.Find(c => c.Id == conversationId).FirstOrDefaultAsync();
        if (conversation == null || !conversation.Participants.Contains(userId))
        {
            return (false, null);
        }

        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = userId,
            Content = request.Content,
            MessageType = request.MessageType,
            Attachments = request.Attachments?.Select(a => new MessageAttachment
            {
                FileName = a.FileName,
                FileUrl = a.FileUrl,
                FileSize = a.FileSize,
                FileType = a.FileType
            }).ToList() ?? new(),
            CustomOffer = request.CustomOffer == null ? null : new CustomOffer
            {
                Price = request.CustomOffer.Price,
                DeliveryDays = request.CustomOffer.DeliveryDays,
                Description = request.CustomOffer.Description
            }
        };

        await _messages.InsertOneAsync(message);

        // Update conversation
        conversation.LastMessage = new LastMessageInfo
        {
            Content = request.MessageType == "text" ? request.Content ?? "" : $"[{request.MessageType}]",
            SenderId = userId,
            Timestamp = DateTime.UtcNow
        };
        conversation.UpdatedAt = DateTime.UtcNow;

        var otherParticipant = conversation.Participants.FirstOrDefault(p => p != userId);
        if (otherParticipant != null)
        {
            if (!conversation.UnreadCount.ContainsKey(otherParticipant))
                conversation.UnreadCount[otherParticipant] = 0;
            conversation.UnreadCount[otherParticipant]++;

            // Send notification
            if (Guid.TryParse(userId, out var userGuid))
            {
                var sender = await _context.Users.FindAsync(userGuid);
                await _notificationService.CreateNotificationAsync(
                    otherParticipant,
                    "new_message",
                    "New Message",
                    $"{sender?.Username ?? "Someone"} sent you a message",
                    new Dictionary<string, object> { { "conversationId", conversationId } }
                );
            }
        }

        await _conversations.ReplaceOneAsync(c => c.Id == conversationId, conversation);

        return (true, new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            Content = message.Content,
            MessageType = message.MessageType,
            CreatedAt = message.CreatedAt
        });
    }
}
