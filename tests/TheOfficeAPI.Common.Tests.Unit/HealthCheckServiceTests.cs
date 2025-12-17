using TheOfficeAPI.Common.Services;

namespace TheOfficeAPI.Common.Tests.Unit;

public class HealthCheckServiceTests
{
    [Fact]
    public void GetLivenessStatus_ReturnsHealthyStatus()
    {
        // Arrange
        var service = new HealthCheckService();

        // Act
        var result = service.GetLivenessStatus();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Healthy", result.Status);
        Assert.Equal("Application is alive", result.Message);
        Assert.True(result.Timestamp <= DateTime.UtcNow);
    }

    [Fact]
    public void GetLivenessStatus_ReturnsCurrentTimestamp()
    {
        // Arrange
        var service = new HealthCheckService();
        var beforeCall = DateTime.UtcNow;

        // Act
        var result = service.GetLivenessStatus();
        var afterCall = DateTime.UtcNow;

        // Assert
        Assert.True(result.Timestamp >= beforeCall);
        Assert.True(result.Timestamp <= afterCall);
    }

    [Fact]
    public void GetHealthStatus_ReturnsHealthyStatus()
    {
        // Arrange
        var service = new HealthCheckService();

        // Act
        var result = service.GetHealthStatus();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Healthy", result.Status);
        Assert.Equal("OK", result.Message);
        Assert.True(result.Timestamp <= DateTime.UtcNow);
    }

    [Fact]
    public void GetReadinessStatus_ReturnsDetailedHealthStatus()
    {
        // Arrange
        var service = new HealthCheckService();

        // Act
        var result = service.GetReadinessStatus();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Healthy", result.Status);
        Assert.Equal("Application is ready to serve traffic", result.Message);
        Assert.True(result.Timestamp <= DateTime.UtcNow);
    }

    [Fact]
    public void GetReadinessStatus_IncludesVersion()
    {
        // Arrange
        var service = new HealthCheckService();

        // Act
        var result = service.GetReadinessStatus();

        // Assert
        Assert.NotNull(result.Version);
        Assert.NotEmpty(result.Version);
    }

    [Fact]
    public void GetReadinessStatus_IncludesUptime()
    {
        // Arrange
        var service = new HealthCheckService();
        Thread.Sleep(100); // Wait a bit to ensure uptime is > 0

        // Act
        var result = service.GetReadinessStatus();

        // Assert
        Assert.True(result.Uptime > TimeSpan.Zero);
        Assert.True(result.Uptime < TimeSpan.FromMinutes(1)); // Should be recent
    }

    [Fact]
    public void GetReadinessStatus_IncludesApplicationComponent()
    {
        // Arrange
        var service = new HealthCheckService();

        // Act
        var result = service.GetReadinessStatus();

        // Assert
        Assert.NotNull(result.Components);
        Assert.True(result.Components.ContainsKey("application"));

        var appComponent = result.Components["application"];
        Assert.Equal("Healthy", appComponent.Status);
        Assert.Equal("Application is running normally", appComponent.Description);
        Assert.NotNull(appComponent.Data);
        Assert.True(appComponent.Data.ContainsKey("startTime"));
        Assert.True(appComponent.Data.ContainsKey("uptime"));
    }

    [Fact]
    public void GetReadinessStatus_IncludesDataServiceComponent()
    {
        // Arrange
        var service = new HealthCheckService();

        // Act
        var result = service.GetReadinessStatus();

        // Assert
        Assert.NotNull(result.Components);
        Assert.True(result.Components.ContainsKey("dataService"));

        var dataComponent = result.Components["dataService"];
        Assert.Equal("Healthy", dataComponent.Status);
        Assert.Equal("In-memory data service is available", dataComponent.Description);
        Assert.NotNull(dataComponent.Data);
        Assert.True(dataComponent.Data.ContainsKey("type"));
        Assert.Equal("In-Memory", dataComponent.Data["type"]);
        Assert.True(dataComponent.Data.ContainsKey("initialized"));
        Assert.Equal(true, dataComponent.Data["initialized"]);
    }

    [Fact]
    public void GetReadinessStatus_UptimeIncreasesOverTime()
    {
        // Arrange
        var service = new HealthCheckService();

        // Act
        var result1 = service.GetReadinessStatus();
        Thread.Sleep(50);
        var result2 = service.GetReadinessStatus();

        // Assert
        Assert.True(result2.Uptime > result1.Uptime);
    }

    [Fact]
    public void MultipleInstances_HaveIndependentStartTimes()
    {
        // Arrange & Act
        var service1 = new HealthCheckService();
        Thread.Sleep(50);
        var service2 = new HealthCheckService();

        var result1 = service1.GetReadinessStatus();
        var result2 = service2.GetReadinessStatus();

        // Assert
        Assert.True(result1.Uptime > result2.Uptime);
    }

    [Fact]
    public void GetHealthStatus_ReturnsCurrentTimestamp()
    {
        // Arrange
        var service = new HealthCheckService();
        var beforeCall = DateTime.UtcNow;

        // Act
        var result = service.GetHealthStatus();
        var afterCall = DateTime.UtcNow;

        // Assert
        Assert.True(result.Timestamp >= beforeCall);
        Assert.True(result.Timestamp <= afterCall);
    }

    [Fact]
    public void GetReadinessStatus_ReturnsCurrentTimestamp()
    {
        // Arrange
        var service = new HealthCheckService();
        var beforeCall = DateTime.UtcNow;

        // Act
        var result = service.GetReadinessStatus();
        var afterCall = DateTime.UtcNow;

        // Assert
        Assert.True(result.Timestamp >= beforeCall);
        Assert.True(result.Timestamp <= afterCall);
    }
}
