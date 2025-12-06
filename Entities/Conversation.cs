namespace FreelancerPlatform.Entities
{
    // Represents a chat thread/room.
    public class Conversation
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Used to sort conversations by the most recent activity.
        public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;

        // List of users in this conversation (usually 2).
        public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();

        // List of messages inside this conversation.
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}