using LogService.Infrastructure.Kafka;
using LogService.Infrastructure.Kafka.Configuration;
using LogService.Infrastructure.MongoDB;
using LogService.Infrastructure.MongoDB.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add configurations
        services.Configure<KafkaSettings>(
            context.Configuration.GetSection("Kafka"));
        services.Configure<MongoSettings>(
            context.Configuration.GetSection("MongoDB"));

        // Add services
        services.AddSingleton<ILogRepository, MongoLogRepository>();
        services.AddHostedService<KafkaConsumer>();
    })
    .Build();

await host.RunAsync();
