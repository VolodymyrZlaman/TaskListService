namespace LogService.Infrastructure.Kafka.Models;

public record TaskListEvent(string EventId, string EventType, string Producer, TaskList Payload)
{
    public string EventId { get; set; } = EventId;
    public string EventType { get; set; } = EventType;
    public string Producer { get; set; } = Producer;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public TaskList Payload { get; set; } = Payload;
    public Dictionary<string, string> Metadata { get; init; } = new();
} 