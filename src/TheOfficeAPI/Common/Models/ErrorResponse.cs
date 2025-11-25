namespace TheOfficeAPI.Common.Models;

/// <summary>
/// Standard error response model for API errors
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Detailed error information (only in development)
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Timestamp of when the error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Request path that caused the error
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Unique trace identifier for debugging
    /// </summary>
    public string? TraceId { get; set; }
}
