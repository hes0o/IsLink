using FreelancerPlatform.Data;
using FreelancerPlatform.Entities;
using FreelancerPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerPlatform.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Client)
                .ThenInclude(u => u.Profile)
                .Include(o => o.Freelancer)
                .Include(o => o.Gig)
                .Include(o => o.Review)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
