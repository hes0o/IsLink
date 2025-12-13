using IsLink.API.Models.MongoDB;
using MongoDB.Driver;

namespace IsLink.API.Services;

public class NotificationService : INotificationService
{
    private readonly IMongoCollection<Notification> _notifications;

    public NotificationService(IMongoDatabase database)
    {
        _notifications = database.GetCollection<Notification>("notifications");
    }

    public async Task<Notification> CreateNotificationAsync(string userId, string type, string title, string message, Dictionary<string, object>? data = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            Data = data ?? new Dictionary<string, object>()
        };

        await _notifications.InsertOneAsync(notification);
        return notification;
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(string userId, int limit, int skip)
    {
        return await _notifications
            .Find(n => n.UserId == userId)
            .SortByDescending(n => n.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return (int)await _notifications.CountDocumentsAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var update = Builders<Notification>.Update
            .Set(n => n.IsRead, true)
            .Set(n => n.ReadAt, DateTime.UtcNow);

        await _notifications.UpdateManyAsync(
            n => n.UserId == userId && !n.IsRead,
            update);
    }
}

