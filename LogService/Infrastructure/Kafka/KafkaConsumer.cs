using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using LogService.Domain.Entities;
using LogService.Infrastructure.Kafka.Configuration;
using LogService.Infrastructure.Kafka.Models;
using LogService.Infrastructure.MongoDB;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogService.Infrastructure.Kafka;

public class KafkaConsumer : IHostedService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogRepository _logRepository;
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly string _topic;
    private Task? _consumeTask;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly JsonSerializerOptions _jsonOptions;

    public KafkaConsumer(
        IOptions<KafkaSettings> settings,
        ILogRepository logRepository,
        ILogger<KafkaConsumer> logger)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = settings.Value.BootstrapServers,
            GroupId = settings.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => logger.LogError("Kafka error: {Error}", e.Reason))
            .Build();

        _topic = settings.Value.Topic;
        _logRepository = logRepository;
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_topic);
        _consumeTask = Task.Run(ConsumeAsync, cancellationToken);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();
        if (_consumeTask != null)
        {
            await _consumeTask;
        }
        
        _consumer.Close();
    }

    private async Task ConsumeAsync()
    {
        try
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(_cancellationTokenSource.Token);
                    if (consumeResult == null) continue;

                    _logger.LogDebug("Received message: {Value}", consumeResult.Message.Value);

                    var taskListEvent = JsonSerializer.Deserialize<TaskListEvent>(
                        consumeResult.Message.Value,
                        _jsonOptions);

                    if (taskListEvent?.Payload == null)
                    {
                        _logger.LogWarning(
                            "Failed to deserialize event or payload is null. Message: {Message}",
                            consumeResult.Message.Value);
                        continue;
                    }

                    var log = new TaskListLog
                    {
                        EventId = taskListEvent.EventId,
                        EventType = taskListEvent.EventType,
                        TaskListId = taskListEvent.Payload.Id,
                        OwnerId = taskListEvent.Payload.OwnerId,
                        CreationDate = taskListEvent.Payload.CreationDate
                    };

                    await _logRepository.SaveLogAsync(log, _cancellationTokenSource.Token);
                    _consumer.Commit(consumeResult);
                    
                    _logger.LogInformation(
                        "Processed event {EventType} for TaskList {TaskListId}. Producer: {Producer}, Timestamp: {Timestamp}",
                        log.EventType,
                        log.TaskListId,
                        taskListEvent.Producer,
                        taskListEvent.Timestamp);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing message");
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in consumer");
        }
    }

    private static string? GetHeaderValue(Headers headers, string key)
    {
        var header = headers.FirstOrDefault(h => h.Key == key);
        return header == null
            ? null
            : Encoding.UTF8.GetString(header.GetValueBytes());
    }
} 