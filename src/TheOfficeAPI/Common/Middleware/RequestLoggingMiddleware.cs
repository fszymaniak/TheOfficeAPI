using System.Diagnostics;

namespace TheOfficeAPI.Common.Middleware;

/// <summary>
/// Middleware for logging HTTP requests and responses with correlation IDs
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Generate or retrieve correlation ID
        var correlationId = context.TraceIdentifier;
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        // Start timing the request
        var stopwatch = Stopwatch.StartNew();

        // Log the incoming request
        _logger.LogInformation(
            "HTTP {Method} {Path} started. CorrelationId: {CorrelationId}, RemoteIP: {RemoteIP}",
            context.Request.Method,
            context.Request.Path,
            correlationId,
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown");

        try
        {
            // Call the next middleware in the pipeline
            await _next(context);

            // Log the completed request
            stopwatch.Stop();
            _logger.LogInformation(
                "HTTP {Method} {Path} completed. StatusCode: {StatusCode}, Duration: {Duration}ms, CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                correlationId);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex,
                "HTTP {Method} {Path} failed. Duration: {Duration}ms, CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                correlationId);

            throw; // Re-throw to let the exception handler middleware deal with it
        }
    }
}

/// <summary>
/// Extension methods for registering the request logging middleware
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// Adds request logging middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
