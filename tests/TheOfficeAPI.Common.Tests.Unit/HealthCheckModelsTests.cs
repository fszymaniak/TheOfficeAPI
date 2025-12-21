using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Common.Tests.Unit;

public class HealthCheckModelsTests
{
    [AllureXunit]
    public void HealthStatus_HasExpectedValues()
    {
        // Assert
        Assert.Equal(0, (int)HealthStatus.Healthy);
        Assert.Equal(1, (int)HealthStatus.Degraded);
        Assert.Equal(2, (int)HealthStatus.Unhealthy);
    }

    [AllureXunit]
    public void HealthCheckResponse_DefaultConstructor_SetsDefaultValues()
    {
        // Act
        var response = new HealthCheckResponse();

        // Assert
        Assert.Equal("Healthy", response.Status);
        Assert.NotEqual(default(DateTime), response.Timestamp);
        Assert.Null(response.Message);
    }

    [AllureXunit]
    public void HealthCheckResponse_TimestampIsSetToUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var response = new HealthCheckResponse();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(response.Timestamp >= beforeCreation);
        Assert.True(response.Timestamp <= afterCreation);
    }

    [AllureXunit]
    public void HealthCheckResponse_CanSetStatus()
    {
        // Arrange
        var response = new HealthCheckResponse();

        // Act
        response.Status = "Unhealthy";

        // Assert
        Assert.Equal("Unhealthy", response.Status);
    }

    [AllureXunit]
    public void HealthCheckResponse_CanSetMessage()
    {
        // Arrange
        var response = new HealthCheckResponse();

        // Act
        response.Message = "Test message";

        // Assert
        Assert.Equal("Test message", response.Message);
    }

    [AllureXunit]
    public void DetailedHealthCheckResponse_DefaultConstructor_SetsDefaultValues()
    {
        // Act
        var response = new DetailedHealthCheckResponse();

        // Assert
        Assert.Equal("Healthy", response.Status);
        Assert.NotEqual(default(DateTime), response.Timestamp);
        Assert.Null(response.Message);
        Assert.NotNull(response.Components);
        Assert.Empty(response.Components);
        Assert.Equal(TimeSpan.Zero, response.Uptime);
        Assert.Equal("1.0.0", response.Version);
    }

    [AllureXunit]
    public void DetailedHealthCheckResponse_CanSetComponents()
    {
        // Arrange
        var response = new DetailedHealthCheckResponse();
        var components = new Dictionary<string, ComponentHealth>
        {
            ["database"] = new ComponentHealth { Status = "Healthy" }
        };

        // Act
        response.Components = components;

        // Assert
        Assert.Equal(components, response.Components);
        Assert.True(response.Components.ContainsKey("database"));
    }

    [AllureXunit]
    public void DetailedHealthCheckResponse_CanSetUptime()
    {
        // Arrange
        var response = new DetailedHealthCheckResponse();
        var uptime = TimeSpan.FromMinutes(5);

        // Act
        response.Uptime = uptime;

        // Assert
        Assert.Equal(uptime, response.Uptime);
    }

    [AllureXunit]
    public void DetailedHealthCheckResponse_CanSetVersion()
    {
        // Arrange
        var response = new DetailedHealthCheckResponse();

        // Act
        response.Version = "2.0.0";

        // Assert
        Assert.Equal("2.0.0", response.Version);
    }

    [AllureXunit]
    public void ComponentHealth_DefaultConstructor_SetsDefaultValues()
    {
        // Act
        var component = new ComponentHealth();

        // Assert
        Assert.Equal("Healthy", component.Status);
        Assert.Null(component.Description);
        Assert.Null(component.Data);
    }

    [AllureXunit]
    public void ComponentHealth_CanSetStatus()
    {
        // Arrange
        var component = new ComponentHealth();

        // Act
        component.Status = "Degraded";

        // Assert
        Assert.Equal("Degraded", component.Status);
    }

    [AllureXunit]
    public void ComponentHealth_CanSetDescription()
    {
        // Arrange
        var component = new ComponentHealth();

        // Act
        component.Description = "Database connection pool is at 90%";

        // Assert
        Assert.Equal("Database connection pool is at 90%", component.Description);
    }

    [AllureXunit]
    public void ComponentHealth_CanSetData()
    {
        // Arrange
        var component = new ComponentHealth();
        var data = new Dictionary<string, object>
        {
            ["connections"] = 90,
            ["maxConnections"] = 100
        };

        // Act
        component.Data = data;

        // Assert
        Assert.Equal(data, component.Data);
        Assert.True(component.Data.ContainsKey("connections"));
        Assert.Equal(90, component.Data["connections"]);
    }

    [AllureXunit]
    public void DetailedHealthCheckResponse_InheritsFromHealthCheckResponse()
    {
        // Arrange & Act
        var detailed = new DetailedHealthCheckResponse
        {
            Status = "Healthy",
            Message = "All systems operational",
            Timestamp = DateTime.UtcNow
        };

        // Assert - Can use as base type
        HealthCheckResponse baseResponse = detailed;
        Assert.Equal("Healthy", baseResponse.Status);
        Assert.Equal("All systems operational", baseResponse.Message);
    }

    [AllureXunit]
    public void ComponentHealth_AllPropertiesCanBeSet()
    {
        // Arrange & Act
        var component = new ComponentHealth
        {
            Status = "Healthy",
            Description = "Service is operating normally",
            Data = new Dictionary<string, object>
            {
                ["latency"] = "5ms",
                ["requestsPerSecond"] = 1000
            }
        };

        // Assert
        Assert.Equal("Healthy", component.Status);
        Assert.Equal("Service is operating normally", component.Description);
        Assert.NotNull(component.Data);
        Assert.Equal(2, component.Data.Count);
        Assert.Equal("5ms", component.Data["latency"]);
        Assert.Equal(1000, component.Data["requestsPerSecond"]);
    }
}
