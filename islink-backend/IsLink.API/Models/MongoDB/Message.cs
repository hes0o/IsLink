using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IsLink.API.Models.MongoDB;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("conversationId")]
    public string ConversationId { get; set; } = string.Empty;

    [BsonElement("senderId")]
    public string SenderId { get; set; } = string.Empty;

    [BsonElement("content")]
    public string? Content { get; set; }

    [BsonElement("messageType")]
    public string MessageType { get; set; } = "text"; // text, image, file, offer, system

    [BsonElement("attachments")]
    public List<MessageAttachment> Attachments { get; set; } = new();

    [BsonElement("customOffer")]
    public CustomOffer? CustomOffer { get; set; }

    [BsonElement("isRead")]
    public bool IsRead { get; set; } = false;

    [BsonElement("readAt")]
    public DateTime? ReadAt { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MessageAttachment
{
    [BsonElement("fileName")]
    public string FileName { get; set; } = string.Empty;

    [BsonElement("fileUrl")]
    public string FileUrl { get; set; } = string.Empty;

    [BsonElement("fileSize")]
    public int? FileSize { get; set; }

    [BsonElement("fileType")]
    public string? FileType { get; set; }
}

public class CustomOffer
{
    [BsonElement("price")]
    public decimal Price { get; set; }

    [BsonElement("deliveryDays")]
    public int DeliveryDays { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = "pending"; // pending, accepted, declined, expired
}

