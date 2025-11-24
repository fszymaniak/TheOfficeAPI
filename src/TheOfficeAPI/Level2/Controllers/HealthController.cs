using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Common.Services;

namespace TheOfficeAPI.Level2.Controllers;

/// <summary>
/// Health check controller for Level 2 (Richardson Maturity Model)
/// </summary>
[ApiController]
[Route("api/v2/health")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    /// <response code="200">Service is healthy</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        var health = _healthCheckService.GetHealthStatus();
        return Ok(health);
    }

    /// <summary>
    /// Liveness probe - checks if the application is running
    /// </summary>
    /// <returns>Liveness status</returns>
    /// <response code="200">Application is alive</response>
    /// <remarks>
    /// This endpoint is used by orchestrators (like Kubernetes) to determine if the application
    /// is still running. If this fails, the container should be restarted.
    /// </remarks>
    [HttpGet("live")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetLiveness()
    {
        var health = _healthCheckService.GetLivenessStatus();
        return Ok(health);
    }

    /// <summary>
    /// Readiness probe - checks if the application is ready to serve traffic
    /// </summary>
    /// <returns>Readiness status with component details</returns>
    /// <response code="200">Application is ready</response>
    /// <remarks>
    /// This endpoint is used by orchestrators (like Kubernetes) to determine if the application
    /// is ready to receive traffic. If this fails, traffic should not be routed to this instance.
    /// </remarks>
    [HttpGet("ready")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetReadiness()
    {
        var health = _healthCheckService.GetReadinessStatus();
        return Ok(health);
    }
}
