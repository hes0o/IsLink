public interface INotificationRepository
{
    Task<List<Notification>> GetForUserAsync(int userId);
    Task SaveChangesAsync();
}
