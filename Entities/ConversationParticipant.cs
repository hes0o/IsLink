using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerPlatform.Entities
{
    // Join entity to link a User to a Conversation.
    public class ConversationParticipant
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }
        [ForeignKey("ConversationId")]
        public Conversation Conversation { get; set; } = null!;

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}