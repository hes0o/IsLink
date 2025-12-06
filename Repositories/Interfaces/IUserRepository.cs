using FreelancerPlatform.Entities;

namespace FreelancerPlatform.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        void AddUser(User user);
        Task<bool> SaveChangesAsync();
    }
}
