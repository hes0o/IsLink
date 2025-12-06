using FreelancerPlatform.Data;
using FreelancerPlatform.Entities;
using FreelancerPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerPlatform.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
