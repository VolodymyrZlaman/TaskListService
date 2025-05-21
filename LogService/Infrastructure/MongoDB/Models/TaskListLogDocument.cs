using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LogService.Infrastructure.MongoDB.Models;

public class TaskListLogDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public string EventId { get; set; }
    public string EventType { get; set; }
    public string TaskListId { get; set; } 
    public string OwnerId { get; set; } 
    public DateTime CreationDate { get; set; }
    public DateTime LoggedAt { get; set; }
} 