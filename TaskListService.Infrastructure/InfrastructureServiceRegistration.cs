using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskListService.Application.Contracts.Infrastructure;
using TaskListService.Infrastructure.Kafka;
using TaskListService.Infrastructure.Kafka.Configuration;

namespace TaskListService.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        
        var kafkaSettings = configuration.GetSection("Kafka").Get<KafkaSettings>();
        if (kafkaSettings == null)
        {
            throw new InvalidOperationException("Kafka settings are not configured");
        }

        services.AddSingleton<IProducer<string, string>>(_ =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers,
                ClientId = kafkaSettings.ClientId,
                EnableIdempotence = kafkaSettings.Producer.EnableIdempotence,
                Acks = Acks.All,
                MessageTimeoutMs = kafkaSettings.Producer.MessageTimeoutMs,
                MessageSendMaxRetries = kafkaSettings.Producer.RetryCount,
                RetryBackoffMs = kafkaSettings.Producer.RetryIntervalMs,
                LingerMs = kafkaSettings.Producer.LingerMs,
                BatchSize = kafkaSettings.Producer.BatchSizeKb * 1024,
                EnableDeliveryReports = true
            };

            return new ProducerBuilder<string, string>(config)
                .Build();
        });

        services.AddSingleton<IKafkaProducer, KafkaProducer>();

        return services;
    }
    
}