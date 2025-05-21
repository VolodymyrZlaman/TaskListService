using Confluent.Kafka;
using TaskService.Domain.Common;

namespace TaskListService.Application.Contracts.Infrastructure;

public interface IKafkaProducer
{
    Task<Result<DeliveryResult<string, string>>> PublishAsync<T>(
        string eventType,
        T payload,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);
}