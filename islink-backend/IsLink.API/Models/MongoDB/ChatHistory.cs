using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IsLink.API.Models.MongoDB;

public class ChatHistory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("sessionId")]
    public string SessionId { get; set; } = string.Empty;

    [BsonElement("userId")]
    public string? UserId { get; set; }

    [BsonElement("messages")]
    public List<ChatMessage> Messages { get; set; } = new();

    [BsonElement("metadata")]
    public ChatMetadata Metadata { get; set; } = new();

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class ChatMessage
{
    [BsonElement("role")]
    public string Role { get; set; } = string.Empty; // user, assistant, system

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ChatMetadata
{
    [BsonElement("userAgent")]
    public string? UserAgent { get; set; }

    [BsonElement("ip")]
    public string? Ip { get; set; }

    [BsonElement("startedAt")]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("lastActivityAt")]
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
}

