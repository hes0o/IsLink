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

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Navigation property
    public List<ChatMessage> Messages { get; set; } = new();
}
