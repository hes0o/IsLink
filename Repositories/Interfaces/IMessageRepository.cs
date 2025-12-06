using FreelancerPlatform.Entities;

namespace FreelancerPlatform.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        Task<Conversation?> GetOrCreateConversationAsync(int userAId, int userBId);
        Task<IEnumerable<Conversation>> GetConversationsForUserAsync(int userId);
        Task<IEnumerable<Message>> GetMessagesForConversationAsync(int conversationId, int userId);
        void AddMessage(Message message);
        Task<bool> SaveChangesAsync();
    }
}
