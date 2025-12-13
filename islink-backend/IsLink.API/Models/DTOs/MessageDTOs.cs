using System.ComponentModel.DataAnnotations;

namespace IsLink.API.Models.DTOs;

// ============================================
// Request DTOs
// ============================================

public class CreateConversationRequest
{
    [Required]
    public string ParticipantId { get; set; } = string.Empty;

    public string? RelatedGig { get; set; }
    public string? RelatedOrder { get; set; }
}

public class SendMessageRequest
{
    public string? Content { get; set; }
    public string MessageType { get; set; } = "text";
    public List<AttachmentDto>? Attachments { get; set; }
    public CustomOfferDto? CustomOffer { get; set; }
}

public class AttachmentDto
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public int? FileSize { get; set; }
    public string? FileType { get; set; }
}

public class CustomOfferDto
{
    public decimal Price { get; set; }
    public int DeliveryDays { get; set; }
    public string? Description { get; set; }
}

// ============================================
// Response DTOs
// ============================================

public class ConversationListResponse
{
    public bool Success { get; set; }
    public List<ConversationDto> Data { get; set; } = new();
}

public class ConversationDto
{
    public string Id { get; set; } = string.Empty;
    public ConversationParticipantDto Participant { get; set; } = new();
    public LastMessageDto? LastMessage { get; set; }
    public int UnreadCount { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ConversationParticipantDto
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public bool IsOnline { get; set; }
}

public class LastMessageDto
{
    public string Content { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class MessageListResponse
{
    public bool Success { get; set; }
    public List<MessageDto> Data { get; set; } = new();
}

public class MessageDto
{
    public string Id { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string MessageType { get; set; } = string.Empty;
    public List<AttachmentDto> Attachments { get; set; } = new();
    public CustomOfferDto? CustomOffer { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

