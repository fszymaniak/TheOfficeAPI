using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Common.Models;
using TheOfficeAPI.Common.Services;
using TheOfficeAPI.Level0.Controllers;

namespace TheOfficeAPI.Level0.Tests.Unit;

public class HealthControllerTests
{
    private readonly HealthController _controller;
    private readonly HealthCheckService _healthCheckService;

    public HealthControllerTests()
    {
        _healthCheckService = new HealthCheckService();
        _controller = new HealthController(_healthCheckService);
    }

    [AllureXunit]
    public void Get_ReturnsOkResult()
    {
        // Act
        var result = _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [AllureXunit]
    public void Get_ReturnsHealthCheckResponse()
    {
        // Act
        var result = _controller.Get() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var healthResponse = Assert.IsType<HealthCheckResponse>(result.Value);
        Assert.Equal("Healthy", healthResponse.Status);
        Assert.Equal("OK", healthResponse.Message);
    }

    [AllureXunit]
    public void GetLiveness_ReturnsOkResult()
    {
        // Act
        var result = _controller.GetLiveness();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [AllureXunit]
    public void GetLiveness_ReturnsLivenessStatus()
    {
        // Act
        var result = _controller.GetLiveness() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var healthResponse = Assert.IsType<HealthCheckResponse>(result.Value);
        Assert.Equal("Healthy", healthResponse.Status);
        Assert.Equal("Application is alive", healthResponse.Message);
    }

    [AllureXunit]
    public void GetReadiness_ReturnsOkResult()
    {
        // Act
        var result = _controller.GetReadiness();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [AllureXunit]
    public void GetReadiness_ReturnsDetailedHealthCheckResponse()
    {
        // Act
        var result = _controller.GetReadiness() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var healthResponse = Assert.IsType<DetailedHealthCheckResponse>(result.Value);
        Assert.Equal("Healthy", healthResponse.Status);
        Assert.Equal("Application is ready to serve traffic", healthResponse.Message);
        Assert.NotNull(healthResponse.Version);
        Assert.NotNull(healthResponse.Components);
        Assert.True(healthResponse.Components.Count > 0);
    }

    [AllureXunit]
    public void GetReadiness_IncludesComponentDetails()
    {
        // Act
        var result = _controller.GetReadiness() as OkObjectResult;
        var healthResponse = result?.Value as DetailedHealthCheckResponse;

        // Assert
        Assert.NotNull(healthResponse);
        Assert.True(healthResponse.Components.ContainsKey("application"));
        Assert.True(healthResponse.Components.ContainsKey("dataService"));

        var appComponent = healthResponse.Components["application"];
        Assert.Equal("Healthy", appComponent.Status);
        Assert.NotNull(appComponent.Description);

        var dataComponent = healthResponse.Components["dataService"];
        Assert.Equal("Healthy", dataComponent.Status);
        Assert.NotNull(dataComponent.Description);
    }

    [AllureXunit]
    public void GetReadiness_IncludesUptime()
    {
        // Arrange
        Thread.Sleep(50); // Ensure some uptime

        // Act
        var result = _controller.GetReadiness() as OkObjectResult;
        var healthResponse = result?.Value as DetailedHealthCheckResponse;

        // Assert
        Assert.NotNull(healthResponse);
        Assert.True(healthResponse.Uptime > TimeSpan.Zero);
    }
}
