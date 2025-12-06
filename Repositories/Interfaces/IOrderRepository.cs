using FreelancerPlatform.Entities;

namespace FreelancerPlatform.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrderByIdAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
