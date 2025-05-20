using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using TaskListService.Application.Contracts.Persistence;
using TaskListService.Persistence.Context;
using TaskListService.Persistence.Repository;

namespace TaskListService.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceService(this IServiceCollection services, IConfiguration configuration)
    {
        var dbProvider = configuration.GetSection("Database")["Provider"];

        switch (dbProvider)
        {
            case "mongoDb":
                ConfigureMongoDb(services, configuration);
                break;
            default:
                throw new InvalidOperationException($"Unsupported database provider: {dbProvider}");
        }

        // Register repositories
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<ITaskListRepository, TaskListRepository>();
        
        return services;
    }

    private static void ConfigureMongoDb(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("MongoDb")["ConnectionString"];
        var databaseName = configuration.GetSection("MongoDb")["DatabaseName"];

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("MongoDB connection string is not configured");
        
        if (string.IsNullOrEmpty(databaseName))
            throw new InvalidOperationException("MongoDB database name is not configured");

        // Register MongoDB client and database
        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });

        // Register database context
        services.AddScoped<IDbContext, MongoDbContext>();
    }
}