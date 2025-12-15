using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TheOfficeAPI.Common.Tests.Unit;

public class SwaggerConfigurationExtensionsTests
{
    [Fact]
    public void AddSwaggerServices_Level0_AddsSwaggerGen()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level0.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var swaggerGenOptions = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(swaggerGenOptions);
    }

    [Fact]
    public void AddSwaggerServices_Level1_AddsSwaggerGen()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level1.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var swaggerGenOptions = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(swaggerGenOptions);
    }

    [Fact]
    public void AddSwaggerServices_Level2_AddsSwaggerGen()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level2.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var swaggerGenOptions = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(swaggerGenOptions);
    }

    [Fact]
    public void AddSwaggerServices_Level3_AddsSwaggerGen()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level3.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var swaggerGenOptions = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(swaggerGenOptions);
    }

    [Fact]
    public void UseSwaggerMiddleware_Level0_ConfiguresSwagger()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        TheOfficeAPI.Level0.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddLogging();
        TheOfficeAPI.Level0.Extensions.SwaggerConfiguration.AddSwaggerServices(builder.Services);
        var app = builder.Build();

        // Act - Should not throw
        TheOfficeAPI.Level0.Extensions.SwaggerConfiguration.UseSwaggerMiddleware(app);

        // Assert - If we get here without exception, the middleware was configured
        Assert.NotNull(app);
    }

    [Fact]
    public void UseSwaggerMiddleware_Level1_ConfiguresSwagger()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddLogging();
        TheOfficeAPI.Level1.Extensions.SwaggerConfiguration.AddSwaggerServices(builder.Services);
        var app = builder.Build();

        // Act - Should not throw
        TheOfficeAPI.Level1.Extensions.SwaggerConfiguration.UseSwaggerMiddleware(app);

        // Assert - If we get here without exception, the middleware was configured
        Assert.NotNull(app);
    }

    [Fact]
    public void UseSwaggerMiddleware_Level2_ConfiguresSwagger()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddLogging();
        TheOfficeAPI.Level2.Extensions.SwaggerConfiguration.AddSwaggerServices(builder.Services);
        var app = builder.Build();

        // Act - Should not throw
        TheOfficeAPI.Level2.Extensions.SwaggerConfiguration.UseSwaggerMiddleware(app);

        // Assert - If we get here without exception, the middleware was configured
        Assert.NotNull(app);
    }

    [Fact]
    public void UseSwaggerMiddleware_Level3_ConfiguresSwagger()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddLogging();
        TheOfficeAPI.Level3.Extensions.SwaggerConfiguration.AddSwaggerServices(builder.Services);
        var app = builder.Build();

        // Act - Should not throw
        TheOfficeAPI.Level3.Extensions.SwaggerConfiguration.UseSwaggerMiddleware(app);

        // Assert - If we get here without exception, the middleware was configured
        Assert.NotNull(app);
    }
}
