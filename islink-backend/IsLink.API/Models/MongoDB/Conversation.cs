using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IsLink.API.Models.MongoDB;

public class Conversation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("participants")]
    public List<string> Participants { get; set; } = new();

    [BsonElement("lastMessage")]
    public LastMessageInfo? LastMessage { get; set; }

    [BsonElement("unreadCount")]
    public Dictionary<string, int> UnreadCount { get; set; } = new();

    [BsonElement("relatedOrder")]
    public string? RelatedOrder { get; set; }

    [BsonElement("relatedGig")]
    public string? RelatedGig { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class LastMessageInfo
{
    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    [BsonElement("senderId")]
    public string SenderId { get; set; } = string.Empty;

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
