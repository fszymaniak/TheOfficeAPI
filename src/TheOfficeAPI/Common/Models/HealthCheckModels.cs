namespace TheOfficeAPI.Common.Models;

/// <summary>
/// Health check status enumeration
/// </summary>
public enum HealthStatus
{
    Healthy,
    Degraded,
    Unhealthy
}

/// <summary>
/// Base health check response
/// </summary>
public class HealthCheckResponse
{
    public string Status { get; set; } = "Healthy";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Message { get; set; }
}

/// <summary>
/// Detailed health check response with component information
/// </summary>
public class DetailedHealthCheckResponse : HealthCheckResponse
{
    public Dictionary<string, ComponentHealth> Components { get; set; } = new();
    public TimeSpan Uptime { get; set; }
    public string Version { get; set; } = "1.0.0";
}

/// <summary>
/// Component health status
/// </summary>
public class ComponentHealth
{
    public string Status { get; set; } = "Healthy";
    public string? Description { get; set; }
    public Dictionary<string, object>? Data { get; set; }
}
