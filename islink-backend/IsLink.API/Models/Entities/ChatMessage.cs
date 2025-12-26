using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

public class ChatMessage
{
    public Guid Id { get; set; }

    public Guid ChatSessionId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty; // "user", "assistant", "system"

    [Required]
    public string Content { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey("ChatSessionId")]
    public ChatSession? ChatSession { get; set; }
}
