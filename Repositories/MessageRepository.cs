using FreelancerPlatform.Data;
using FreelancerPlatform.Entities;
using FreelancerPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerPlatform.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;
        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Conversation?> GetOrCreateConversationAsync(int userAId, int userBId)
        {
            var conv = await _context.Conversations
                .Include(c => c.Participants).ThenInclude(p => p.User).ThenInclude(u => u.Profile)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c =>
                    c.Participants.Any(p => p.UserId == userAId) &&
                    c.Participants.Any(p => p.UserId == userBId));

            if (conv != null) return conv;

            conv = new Conversation
            {
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow
            };

            _context.Conversations.Add(conv);
            await _context.SaveChangesAsync();

            _context.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = conv.Id,
                UserId = userAId
            });

            _context.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = conv.Id,
                UserId = userBId
            });

            await _context.SaveChangesAsync();

            return conv;
        }

        public async Task<IEnumerable<Conversation>> GetConversationsForUserAsync(int userId)
        {
            return await _context.Conversations
                .Include(c => c.Participants).ThenInclude(p => p.User).ThenInclude(u => u.Profile)
                .Where(c => c.Participants.Any(p => p.UserId == userId))
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesForConversationAsync(int conversationId, int userId)
        {
            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (!isParticipant)
                return Enumerable.Empty<Message>();

            return await _context.Messages
                .Include(m => m.Sender).ThenInclude(u => u.Profile)
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
