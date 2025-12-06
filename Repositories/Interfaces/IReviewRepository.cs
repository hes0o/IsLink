using FreelancerPlatform.Entities;

namespace FreelancerPlatform.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetReviewsForGigAsync(int gigId);
        Task<bool> HasReviewAsync(int orderId);
        void AddReview(Review review);
        Task<bool> SaveChangesAsync();
    }
}
