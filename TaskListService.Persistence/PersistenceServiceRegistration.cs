using Microsoft.Extensions.DependencyInjection;
using TaskListService.Application.Contracts.Persistence;
using TaskListService.Persistence.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace TaskListService.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistanceService(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("MongoDb")["ConnectionString"];
        var databaseName = configuration.GetSection("MongoDb")["DatabaseName"];
        
        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString ));
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });

        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<ITaskListRepository, TaskListRepository>();
        
        return services;
    }
}