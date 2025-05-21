using LogService.Domain.Entities;
using LogService.Infrastructure.MongoDB.Configuration;
using LogService.Infrastructure.MongoDB.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LogService.Infrastructure.MongoDB;

public interface ILogRepository
{
    Task SaveLogAsync(TaskListLog log, CancellationToken cancellationToken = default);
}

public class MongoLogRepository : ILogRepository
{
    private readonly IMongoCollection<TaskListLogDocument> _collection;
    private readonly ILogger<MongoLogRepository> _logger;

    public MongoLogRepository(IOptions<MongoSettings> settings, ILogger<MongoLogRepository> logger)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<TaskListLogDocument>(settings.Value.CollectionName);
        _logger = logger;
    }

    public async Task SaveLogAsync(TaskListLog log, CancellationToken cancellationToken = default)
    {
        try
        {
            var document = new TaskListLogDocument
            {
                EventId = log.EventId,
                EventType = log.EventType,
                TaskListId = log.TaskListId,
                OwnerId = log.OwnerId,
                CreationDate = log.CreationDate,
                LoggedAt = log.LoggedAt
            };

            await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
            
            _logger.LogInformation(
                "Saved log for TaskList {TaskListId}, Event: {EventType}",
                log.TaskListId,
                log.EventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error saving log for TaskList {TaskListId}",
                log.TaskListId);
            throw;
        }
    }
} 