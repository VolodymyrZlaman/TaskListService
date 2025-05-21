using System.Net;
using TaskListService.Application.Contracts.Infrastructure;
using TaskService.Domain.Common;

namespace TaskListService.API.Middleware;

public class UserValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
    {
        // Skip validation for endpoints that don't require authentication
        if (IsPublicEndpoint(context))
        {
            await next(context);
            return;
        }

        if (!currentUserService.IsAuthenticated)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(Result.Failure("User ID is required"));
            return;
        }

        await next(context);
    }

    private static bool IsPublicEndpoint(HttpContext context)
    {
        // Add your logic to determine public endpoints
        // For example, swagger, health checks, etc.
        return context.Request.Path.StartsWithSegments("/swagger") ||
               context.Request.Path.StartsWithSegments("/health") ||
               context.Request.Path.StartsWithSegments("/scalar") ||
               context.Request.Path.StartsWithSegments("/openapi");
    }
} 