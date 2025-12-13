using IsLink.API.Models.DTOs;

namespace IsLink.API.Services;

public interface IMessageService
{
    Task<ConversationListResponse> GetConversationsAsync(string userId);
    Task<(bool Success, ConversationDto? Data)> GetOrCreateConversationAsync(string userId, CreateConversationRequest request);
    Task<MessageListResponse> GetMessagesAsync(string userId, string conversationId, int limit, DateTime? before);
    Task<(bool Success, MessageDto? Data)> SendMessageAsync(string userId, string conversationId, SendMessageRequest request);
}

