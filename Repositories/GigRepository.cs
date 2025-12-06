using Microsoft.EntityFrameworkCore;
using FreelancerPlatform.Data;
using FreelancerPlatform.Entities;
using FreelancerPlatform.Repositories.Interfaces;

namespace FreelancerPlatform.Repositories
{
    // Implementation of the IGigRepository interface.
    public class GigRepository : IGigRepository
    {
        private readonly AppDbContext _context;

        public GigRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Gig>> GetGigsAsync()
        {
            // Fetch gigs with related data (Category, Freelancer Profile, Images, Tags).
            return await _context.Gigs
                .Include(g => g.Category)
                .Include(g => g.Freelancer)
                .ThenInclude(u => u.Profile)
                .Include(g => g.Images) // Load images
                .Include(g => g.Tags)   // Load tags
                .Include(g => g.Packages)
                .ToListAsync();
        }

        public async Task<Gig?> GetGigByIdAsync(int id)
        {
            return await _context.Gigs
                .Include(g => g.Category)
                .Include(g => g.Freelancer)
                .ThenInclude(u => u.Profile)
                .Include(g => g.Images)
                .Include(g => g.Tags)
                .Include(g => g.Packages)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public void AddGig(Gig gig)
        {
            _context.Gigs.Add(gig);
        }

        public void DeleteGig(Gig gig)
        {
            _context.Gigs.Remove(gig);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}