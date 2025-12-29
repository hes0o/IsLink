using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

public class Conversation
{
    [Key]
    public Guid Id { get; set; }

    public string? RelatedGigId { get; set; } // Can be null if generic convo
    public string? RelatedOrderId { get; set; } // Can be null

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Snapshot of last message for list view performance
    public string? LastMessageContent { get; set; }
    public DateTime? LastMessageTimestamp { get; set; }
    public string? LastMessageSenderId { get; set; }

    // Navigation properties
    public virtual ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
