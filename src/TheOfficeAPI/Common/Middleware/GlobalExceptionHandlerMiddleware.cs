using System.Net;
using System.Text.Json;
using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Common.Middleware;

/// <summary>
/// Global exception handler middleware for consistent error responses
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Path = context.Request.Path,
            TraceId = context.TraceIdentifier
        };

        // Determine status code and message based on exception type
        switch (exception)
        {
            case ArgumentException:
            case ArgumentNullException:
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Invalid request parameters";
                break;

            case KeyNotFoundException:
                errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = "The requested resource was not found";
                break;

            case UnauthorizedAccessException:
                errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Unauthorized access";
                break;

            case InvalidOperationException:
                errorResponse.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse.Message = "Operation cannot be completed in the current state";
                break;

            case NotImplementedException:
                errorResponse.StatusCode = (int)HttpStatusCode.NotImplemented;
                errorResponse.Message = "This functionality is not yet implemented";
                break;

            case TimeoutException:
                errorResponse.StatusCode = (int)HttpStatusCode.RequestTimeout;
                errorResponse.Message = "The request timed out";
                break;

            default:
                errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "An internal server error occurred";
                break;
        }

        // Include detailed error information only in development environment
        if (_environment.IsDevelopment())
        {
            errorResponse.Details = exception.ToString();
        }
        else
        {
            // In production, only include the exception message for known exception types
            if (exception is ArgumentException ||
                exception is ArgumentNullException ||
                exception is KeyNotFoundException ||
                exception is InvalidOperationException)
            {
                errorResponse.Details = exception.Message;
            }
        }

        context.Response.StatusCode = errorResponse.StatusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(errorResponse, options);
        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Extension methods for registering the global exception handler middleware
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    /// <summary>
    /// Adds global exception handling middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
