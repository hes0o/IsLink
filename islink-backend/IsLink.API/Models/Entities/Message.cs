using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsLink.API.Models.Entities;

public class Message
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Conversation")]
    public Guid ConversationId { get; set; }
    public virtual Conversation Conversation { get; set; } = null!;

    [ForeignKey("Sender")]
    public Guid SenderId { get; set; }
    public virtual User Sender { get; set; } = null!;

    [Required]
    public string Content { get; set; } = string.Empty;

    public string MessageType { get; set; } = "text"; // text, image, file, offer

    // JSON stored as string for simplicity in Postgres (mapped to jsonb in OnModelCreating if needed, or just string)
    // Storing basic structure for Attachments and CustomOffer
    [Column(TypeName = "jsonb")]
    public string? AttachmentsJson { get; set; }

    [Column(TypeName = "jsonb")]
    public string? CustomOfferJson { get; set; }

    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
