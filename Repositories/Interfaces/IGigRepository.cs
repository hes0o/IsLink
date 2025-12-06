using FreelancerPlatform.Entities;

namespace FreelancerPlatform.Repositories.Interfaces
{
    // Interface defining the contract for Gig database operations.
    public interface IGigRepository
    {
        // Get all gigs from the database.
        Task<IEnumerable<Gig>> GetGigsAsync();

        // Get a single gig by its ID.
        Task<Gig?> GetGigByIdAsync(int id);

        // Add a new gig to the database.
        void AddGig(Gig gig);

        // Delete an existing gig.
        void DeleteGig(Gig gig);

        // Save changes to the database.
        Task<bool> SaveChangesAsync();
    }
}