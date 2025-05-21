using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskListService.Application.Contracts.Infrastructure;
using TaskListService.Infrastructure.Kafka.Configuration;
using TaskListService.Infrastructure.Kafka.Models;
using TaskService.Domain.Common;

namespace TaskListService.Infrastructure.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaSettings _settings;
    private readonly string _topic;
    private bool _disposed;

    public KafkaProducer(
        IProducer<string, string> producer,
        IOptions<KafkaSettings> settings)
    {
        _producer = producer;
        _settings = settings.Value;
        _topic = _settings.Topics["TaskList"];
    }

    public async Task<Result<DeliveryResult<string, string>>> PublishAsync<T>(
        string eventType,
        T payload,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var kafkaEvent = new KafkaEvent<T>
            {
                EventType = eventType,
                Producer = _settings.ClientId,
                Payload = payload,
                Metadata = metadata ?? new Dictionary<string, string>()
            };

            var headers = new Headers
            {
                { "EventId", Encoding.UTF8.GetBytes(kafkaEvent.EventId) },
                { "EventType", Encoding.UTF8.GetBytes(kafkaEvent.EventType) },
                { "Producer", Encoding.UTF8.GetBytes(kafkaEvent.Producer) },
                { "Timestamp", Encoding.UTF8.GetBytes(kafkaEvent.Timestamp.ToString("O")) }
            };

            foreach (var meta in kafkaEvent.Metadata)
            {
                headers.Add(meta.Key, Encoding.UTF8.GetBytes(meta.Value));
            }

            var message = new Message<string, string>
            {
                Key = kafkaEvent.EventId,
                Value = JsonSerializer.Serialize(kafkaEvent),
                Headers = headers
            };

            var deliveryResult = await _producer.ProduceAsync(_topic, message, cancellationToken);

            return Result<DeliveryResult<string, string>>.Success(deliveryResult);
        }
        catch (ProduceException<string, string> ex)
        {
            return Result<DeliveryResult<string, string>>.Failure(
                $"Failed to publish event. Error: {ex.Error.Reason}");
        }
        catch (Exception ex)
        {
            return Result<DeliveryResult<string, string>>.Failure(
                "Unexpected error publishing event");
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();
        }
        catch (Exception)
        {
            // ignored
        }

        _disposed = true;
    }
}