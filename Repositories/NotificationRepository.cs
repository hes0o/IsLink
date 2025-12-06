using Microsoft.EntityFrameworkCore;
using FreelancerPlatform.Data;
using FreelancerPlatform.Entities;
using FreelancerPlatform.Repositories.Interfaces;

namespace FreelancerPlatform.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetForUserAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}