using System.Reflection;
using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Common.Services;

/// <summary>
/// Service for performing application health checks
/// </summary>
public class HealthCheckService
{
    private readonly DateTime _startTime;
    private readonly string _version;

    public HealthCheckService()
    {
        _startTime = DateTime.UtcNow;
        _version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
    }

    /// <summary>
    /// Performs a basic liveness check - is the application running?
    /// </summary>
    public HealthCheckResponse GetLivenessStatus()
    {
        return new HealthCheckResponse
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Message = "Application is alive"
        };
    }

    /// <summary>
    /// Performs a readiness check - is the application ready to serve traffic?
    /// </summary>
    public DetailedHealthCheckResponse GetReadinessStatus()
    {
        var response = new DetailedHealthCheckResponse
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Message = "Application is ready to serve traffic",
            Uptime = DateTime.UtcNow - _startTime,
            Version = _version,
            Components = new Dictionary<string, ComponentHealth>
            {
                ["application"] = new ComponentHealth
                {
                    Status = "Healthy",
                    Description = "Application is running normally",
                    Data = new Dictionary<string, object>
                    {
                        ["startTime"] = _startTime,
                        ["uptime"] = (DateTime.UtcNow - _startTime).ToString()
                    }
                },
                ["dataService"] = new ComponentHealth
                {
                    Status = "Healthy",
                    Description = "In-memory data service is available",
                    Data = new Dictionary<string, object>
                    {
                        ["type"] = "In-Memory",
                        ["initialized"] = true
                    }
                }
            }
        };

        return response;
    }

    /// <summary>
    /// Gets basic health status for backwards compatibility
    /// </summary>
    public HealthCheckResponse GetHealthStatus()
    {
        return new HealthCheckResponse
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Message = "OK"
        };
    }
}
