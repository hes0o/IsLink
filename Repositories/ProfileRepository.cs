using Microsoft.EntityFrameworkCore;
using FreelancerPlatform.Data;      
using FreelancerPlatform.Entities;   
using FreelancerPlatform.Repositories.Interfaces; 

public class ProfileRepository : IProfileRepository
{
    private readonly AppDbContext _context;

    public ProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Profile?> GetByUserIdAsync(int userId)
    {
        return await _context.Profiles
            .Include(p => p.Skills)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
