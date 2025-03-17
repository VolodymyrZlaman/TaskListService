using Microsoft.Extensions.DependencyInjection;
using TaskListService.Application.Contracts.Aplication;

namespace TaskListService.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddScoped<ITaskListService, Services.TaskListService>();
        return services;
    }
}