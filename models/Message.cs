using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerPlatform.Entities
{
    // Represents a text message sent by a user.
    public class Message
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }
        [ForeignKey("ConversationId")]
        public Conversation Conversation { get; set; } = null!;

        // The user who sent the message.
        public int SenderId { get; set; }
        [ForeignKey("SenderId")]
        public User Sender { get; set; } = null!;

        [Required]
        public string Content { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false; // Status if the message was read.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}