using TaskListService.API.Middleware;

namespace TaskListService.API.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseUserValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserValidationMiddleware>();
    }
} 