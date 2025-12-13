using IsLink.API.Models.MongoDB;

namespace IsLink.API.Services;

public interface INotificationService
{
    Task<Notification> CreateNotificationAsync(string userId, string type, string title, string message, Dictionary<string, object>? data = null);
    Task<List<Notification>> GetUserNotificationsAsync(string userId, int limit, int skip);
    Task<int> GetUnreadCountAsync(string userId);
    Task MarkAllAsReadAsync(string userId);
}

