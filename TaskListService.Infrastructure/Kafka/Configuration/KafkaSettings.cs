namespace TaskListService.Infrastructure.Kafka.Configuration;

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string ClientId { get; set; } 
    public ProducerSettings Producer { get; set; } = new();
    public Dictionary<string, string> Topics { get; set; } = new();
}

public class ProducerSettings
{
    public int MessageTimeoutMs { get; set; } = 30000;
    public bool EnableIdempotence { get; set; } = true;
    public string Acks { get; set; } = "all";
    public int RetryCount { get; set; } = 3;
    public int RetryIntervalMs { get; set; } = 1000;
    public int BatchSizeKb { get; set; } = 1024;
    public int LingerMs { get; set; } = 5;
} 