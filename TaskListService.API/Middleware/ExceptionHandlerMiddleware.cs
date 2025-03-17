using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TaskListService.Application.Exceptions;

namespace TaskListService.API.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred.");
            await ConvertException(context, ex);
        }
    }

    private Task ConvertException(HttpContext context, Exception exception)
    {
        HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        string result;

        switch (exception)
        {
            case ApplicationException applicationException:
                httpStatusCode = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new { message = applicationException.Message });
                break;
            case BadRequestException badRequestException:
                httpStatusCode = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new { message = badRequestException.Message });
                break;
            case NotFoundException:
                httpStatusCode = HttpStatusCode.NotFound;
                result = JsonSerializer.Serialize(new { message = "Resource not found" });
                break;
            case UnauthorizedAccessException:
                httpStatusCode = HttpStatusCode.Unauthorized;
                result = JsonSerializer.Serialize(new { message = "Unauthorized access" });
                break;
            case ValidationException validationException:
                httpStatusCode = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new
                {
                    message = "Validation failed",
                    errors = validationException.ValdationErrors
                });
                break;
            case MongoWriteException mongoWriteException:
                httpStatusCode = HttpStatusCode.InternalServerError;
                result = JsonSerializer.Serialize(new { message = "TaskList with the same id already exists." });
                break;
            default:
                httpStatusCode = HttpStatusCode.InternalServerError;
                result = JsonSerializer.Serialize(new { message = "An unexpected error occurred" });
                break;
        }

        context.Response.StatusCode = (int)httpStatusCode;
        return context.Response.WriteAsync(result);
    }
}