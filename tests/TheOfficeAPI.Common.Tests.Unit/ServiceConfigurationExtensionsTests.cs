using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TheOfficeAPI.Common.Enums;
using TheOfficeAPI.Common.Extensions;
using TheOfficeAPI.Common.Services;

namespace TheOfficeAPI.Common.Tests.Unit;

public class ServiceConfigurationExtensionsTests
{
    [Fact]
    public void ConfigureServices_WithLevel0_RegistersHealthCheckService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.ConfigureServices(MaturityLevel.Level0);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckService = serviceProvider.GetService<HealthCheckService>();
        Assert.NotNull(healthCheckService);
    }

    [Fact]
    public void ConfigureServices_WithLevel1_RegistersHealthCheckService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.ConfigureServices(MaturityLevel.Level1);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckService = serviceProvider.GetService<HealthCheckService>();
        Assert.NotNull(healthCheckService);
    }

    [Fact]
    public void ConfigureServices_WithLevel2_RegistersHealthCheckService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.ConfigureServices(MaturityLevel.Level2);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckService = serviceProvider.GetService<HealthCheckService>();
        Assert.NotNull(healthCheckService);
    }

    [Fact]
    public void ConfigureServices_WithLevel3_RegistersHealthCheckService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.ConfigureServices(MaturityLevel.Level3);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckService = serviceProvider.GetService<HealthCheckService>();
        Assert.NotNull(healthCheckService);
    }

    [Fact]
    public void ConfigureServices_WithLevel0_AddsControllers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.ConfigureServices(MaturityLevel.Level0);

        // Assert
        Assert.Contains(services, s => s.ServiceType.Name == "ApplicationPartManager");
    }

    [Fact]
    public void ConfigureServices_WithNull_RegistersHealthCheckService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.ConfigureServices(null);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckService = serviceProvider.GetService<HealthCheckService>();
        Assert.NotNull(healthCheckService);
    }

    [Fact]
    public void ConfigureServices_RegistersAllLevelServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.ConfigureServices(MaturityLevel.Level3);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Should register services from all levels
        var level0Service = serviceProvider.GetService<TheOfficeAPI.Level0.Services.TheOfficeService>();
        var level1Service = serviceProvider.GetService<TheOfficeAPI.Level1.Services.TheOfficeService>();
        var level2Service = serviceProvider.GetService<TheOfficeAPI.Level2.Services.TheOfficeService>();
        var level3Service = serviceProvider.GetService<TheOfficeAPI.Level3.Services.TheOfficeService>();

        Assert.NotNull(level0Service);
        Assert.NotNull(level1Service);
        Assert.NotNull(level2Service);
        Assert.NotNull(level3Service);
    }

    [Fact]
    public void ConfigurePipeline_WithLevel0_ConfiguresPipelineWithoutException()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddLogging();
        builder.Services.ConfigureServices(MaturityLevel.Level0);
        var app = builder.Build();

        // Act & Assert - Should not throw
        app.ConfigurePipeline(MaturityLevel.Level0);
        Assert.NotNull(app);
    }

    [Fact]
    public void ConfigurePipeline_WithLevel1_ConfiguresPipelineWithoutException()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddLogging();
        builder.Services.ConfigureServices(MaturityLevel.Level1);
        var app = builder.Build();

        // Act & Assert - Should not throw
        app.ConfigurePipeline(MaturityLevel.Level1);
        Assert.NotNull(app);
    }

    [Fact]
    public void ConfigurePipeline_WithLevel2_ConfiguresPipelineWithoutException()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddLogging();
        builder.Services.ConfigureServices(MaturityLevel.Level2);
        var app = builder.Build();

        // Act & Assert - Should not throw
        app.ConfigurePipeline(MaturityLevel.Level2);
        Assert.NotNull(app);
    }

    [Fact]
    public void ConfigurePipeline_WithLevel3_ConfiguresPipelineWithoutException()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddLogging();
        builder.Services.ConfigureServices(MaturityLevel.Level3);
        var app = builder.Build();

        // Act & Assert - Should not throw
        app.ConfigurePipeline(MaturityLevel.Level3);
        Assert.NotNull(app);
    }

    [Fact]
    public void ConfigurePipeline_WithNull_ConfiguresPipelineWithoutException()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddLogging();
        builder.Services.ConfigureServices(null);
        var app = builder.Build();

        // Act & Assert - Should not throw
        app.ConfigurePipeline(null);
        Assert.NotNull(app);
    }
}
