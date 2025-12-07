using FreelancerPlatform.Data;
using FreelancerPlatform.Entities;
using FreelancerPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                    .ThenInclude(u => u.Profile)
                .Include(o => o.Gig)
                .Include(o => o.Review)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersForClientAsync(int clientId)
        {
            return await _context.Orders
                .Include(o => o.Gig)
                .Include(o => o.Client).ThenInclude(u => u.Profile)
                .Include(o => o.Freelancer).ThenInclude(u => u.Profile)
                .Where(o => o.ClientId == clientId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersForFreelancerAsync(int freelancerId)
        {
            return await _context.Orders
                .Include(o => o.Gig)
                .Include(o => o.Client).ThenInclude(u => u.Profile)
                .Include(o => o.Freelancer).ThenInclude(u => u.Profile)
                .Where(o => o.FreelancerId == freelancerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
