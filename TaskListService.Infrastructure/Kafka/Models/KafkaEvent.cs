using System.Text.Json.Serialization;

namespace TaskListService.Infrastructure.Kafka.Models;

public record KafkaEvent<T>
{
    public string EventId { get; init; } = Guid.NewGuid().ToString();
    public string EventType { get; init; } 
    public string Producer { get; init; } 
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public T Payload { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = new();
} 