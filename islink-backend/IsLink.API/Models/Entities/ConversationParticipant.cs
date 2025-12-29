using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

public class ConversationParticipant
{
    public Guid Id { get; set; }

    [ForeignKey("Conversation")]
    public Guid ConversationId { get; set; }
    public virtual Conversation Conversation { get; set; } = null!;

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public int UnreadCount { get; set; } = 0;
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
