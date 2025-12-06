using FreelancerPlatform.Entities;

public interface INotificationRepository
{
    Task<List<Notification>> GetForUserAsync(int userId);
    Task SaveChangesAsync();
}
