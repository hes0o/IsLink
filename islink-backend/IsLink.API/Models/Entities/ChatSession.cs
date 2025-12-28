using System.ComponentModel.DataAnnotations;

namespace IsLink.API.Models.Entities;

public class ChatSession
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string SessionId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string UserId { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Title { get; set; } = "New Chat";

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Stores the final recommendations as a JSON string when the session is complete.
    /// </summary>
    public string? RecommendationsJson { get; set; }

    // Navigation property
    public List<ChatMessage> Messages { get; set; } = new();
}
