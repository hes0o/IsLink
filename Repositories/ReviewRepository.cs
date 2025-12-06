using FreelancerPlatform.Data;
using FreelancerPlatform.Entities;
using FreelancerPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerPlatform.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;
        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetReviewsForGigAsync(int gigId)
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .ThenInclude(o => o.Client)
                .ThenInclude(c => c.Profile)
                .Where(r => r.Order.GigId == gigId)
                .ToListAsync();
        }

        public async Task<bool> HasReviewAsync(int orderId)
        {
            return await _context.Reviews.AnyAsync(r => r.OrderId == orderId);
        }

        public void AddReview(Review review)
        {
            _context.Reviews.Add(review);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
