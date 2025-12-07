using FreelancerPlatform.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreelancerPlatform.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        // Get a single order by Id (with related data)
        Task<Order?> GetOrderByIdAsync(int id);

        // Get all orders where the current user is the Client (Buyer)
        Task<IEnumerable<Order>> GetOrdersForClientAsync(int clientId);

        // Get all orders where the current user is the Freelancer (Seller)
        Task<IEnumerable<Order>> GetOrdersForFreelancerAsync(int freelancerId);

        // Add a new order
        void AddOrder(Order order);

        // Save changes
        Task<bool> SaveChangesAsync();
    }
}
