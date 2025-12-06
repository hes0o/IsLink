namespace FreelancerPlatform.DTOs
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public DateTime LastMessageAt { get; set; }
        public List<string> Participants { get; set; } = new();
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateMessageDto
    {
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
