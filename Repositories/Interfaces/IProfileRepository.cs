namespace FreelancerPlatform.Repositories.Interfaces
{
    public interface IProfileRepository
    {
        Task<Profile?> GetByUserIdAsync(int userId);
        Task SaveChangesAsync();
    }
}
