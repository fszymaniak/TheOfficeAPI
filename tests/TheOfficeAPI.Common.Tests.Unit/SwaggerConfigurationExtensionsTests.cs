using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TheOfficeAPI.Common.Tests.Unit;

public class SwaggerConfigurationExtensionsTests
{
    [AllureXunit]
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

    [AllureXunit]
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

    [AllureXunit]
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

    [AllureXunit]
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

    [AllureXunit]
    public void AddSwaggerServices_Level0_ConfiguresAllApiVersions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level0.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var optionsConfigurator = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(optionsConfigurator);

        var options = new SwaggerGenOptions();
        optionsConfigurator.Configure(options);

        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v0");
        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v1");
        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v2");
        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v3");
    }

    [AllureXunit]
    public void AddSwaggerServices_Level1_ConfiguresAllApiVersions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level1.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var optionsConfigurator = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(optionsConfigurator);

        var options = new SwaggerGenOptions();
        optionsConfigurator.Configure(options);

        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v0");
        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v1");
        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v2");
        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v3");
    }

    [AllureXunit]
    public void AddSwaggerServices_Level2_ConfiguresAllApiVersions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level2.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var optionsConfigurator = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(optionsConfigurator);

        var options = new SwaggerGenOptions();
        optionsConfigurator.Configure(options);

        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v0");
        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v1");
        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v2");
        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v3");
    }

    [AllureXunit]
    public void AddSwaggerServices_Level3_ConfiguresAllApiVersions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level3.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var optionsConfigurator = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(optionsConfigurator);

        var options = new SwaggerGenOptions();
        optionsConfigurator.Configure(options);

        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v0");
        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v1");
        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v2");
        Assert.Contains(options.SwaggerGeneratorOptions.SwaggerDocs, d => d.Key == "v3");
    }

    [AllureXunit]
    public void AddSwaggerServices_Level0_ConfiguresV0DocInfo()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level0.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var optionsConfigurator = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(optionsConfigurator);

        var options = new SwaggerGenOptions();
        optionsConfigurator.Configure(options);

        var v0Doc = options.SwaggerGeneratorOptions.SwaggerDocs["v0"];
        Assert.Equal("The Office API - Level 0", v0Doc.Title);
        Assert.Equal("v0", v0Doc.Version);
        Assert.Equal("Richardson Maturity Model Level 0 implementation", v0Doc.Description);
    }

    [AllureXunit]
    public void AddSwaggerServices_Level1_ConfiguresV1DocInfo()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level1.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var optionsConfigurator = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(optionsConfigurator);

        var options = new SwaggerGenOptions();
        optionsConfigurator.Configure(options);

        var v1Doc = options.SwaggerGeneratorOptions.SwaggerDocs["v1"];
        Assert.Equal("The Office API - Level 1", v1Doc.Title);
        Assert.Equal("v1", v1Doc.Version);
        Assert.Contains("Level 1", v1Doc.Description);
    }

    [AllureXunit]
    public void AddSwaggerServices_Level2_ConfiguresV2DocInfo()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level2.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var optionsConfigurator = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(optionsConfigurator);

        var options = new SwaggerGenOptions();
        optionsConfigurator.Configure(options);

        var v2Doc = options.SwaggerGeneratorOptions.SwaggerDocs["v2"];
        Assert.Equal("The Office API - Level 2", v2Doc.Title);
        Assert.Equal("v2", v2Doc.Version);
        Assert.Contains("Level 2", v2Doc.Description);
    }

    [AllureXunit]
    public void AddSwaggerServices_Level3_ConfiguresV3DocInfo()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level3.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var optionsConfigurator = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(optionsConfigurator);

        var options = new SwaggerGenOptions();
        optionsConfigurator.Configure(options);

        var v3Doc = options.SwaggerGeneratorOptions.SwaggerDocs["v3"];
        Assert.Equal("The Office API - Level 3", v3Doc.Title);
        Assert.Equal("v3", v3Doc.Version);
        Assert.Contains("HATEOAS", v3Doc.Description);
    }

    [AllureXunit]
    public void AddSwaggerServices_Level0_ConfiguresDocInclusionPredicate()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level0.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var optionsConfigurator = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(optionsConfigurator);

        var options = new SwaggerGenOptions();
        optionsConfigurator.Configure(options);

        // The DocInclusionPredicate should be set
        Assert.NotNull(options.SwaggerGeneratorOptions.DocInclusionPredicate);
    }

    [AllureXunit]
    public void AddSwaggerServices_Level1_ConfiguresDocInclusionPredicate()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level1.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var optionsConfigurator = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(optionsConfigurator);

        var options = new SwaggerGenOptions();
        optionsConfigurator.Configure(options);

        Assert.NotNull(options.SwaggerGeneratorOptions.DocInclusionPredicate);
    }

    [AllureXunit]
    public void AddSwaggerServices_Level2_ConfiguresDocInclusionPredicate()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level2.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var optionsConfigurator = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(optionsConfigurator);

        var options = new SwaggerGenOptions();
        optionsConfigurator.Configure(options);

        Assert.NotNull(options.SwaggerGeneratorOptions.DocInclusionPredicate);
    }

    [AllureXunit]
    public void AddSwaggerServices_Level3_ConfiguresDocInclusionPredicate()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level3.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var optionsConfigurator = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();
        Assert.NotNull(optionsConfigurator);

        var options = new SwaggerGenOptions();
        optionsConfigurator.Configure(options);

        Assert.NotNull(options.SwaggerGeneratorOptions.DocInclusionPredicate);
    }

    [AllureXunit]
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

    [AllureXunit]
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

    [AllureXunit]
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

    [AllureXunit]
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

    [AllureXunit]
    public void AddSwaggerServices_Level0_RegistersMultipleSwaggerGenOptionsConfigurators()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level0.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var swaggerGenServices = services.Where(s =>
            s.ServiceType == typeof(IConfigureOptions<SwaggerGenOptions>)).ToList();
        Assert.NotEmpty(swaggerGenServices);
    }

    [AllureXunit]
    public void AddSwaggerServices_Level1_RegistersMultipleSwaggerGenOptionsConfigurators()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level1.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var swaggerGenServices = services.Where(s =>
            s.ServiceType == typeof(IConfigureOptions<SwaggerGenOptions>)).ToList();
        Assert.NotEmpty(swaggerGenServices);
    }

    [AllureXunit]
    public void AddSwaggerServices_Level2_RegistersMultipleSwaggerGenOptionsConfigurators()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level2.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var swaggerGenServices = services.Where(s =>
            s.ServiceType == typeof(IConfigureOptions<SwaggerGenOptions>)).ToList();
        Assert.NotEmpty(swaggerGenServices);
    }

    [AllureXunit]
    public void AddSwaggerServices_Level3_RegistersMultipleSwaggerGenOptionsConfigurators()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        TheOfficeAPI.Level3.Extensions.SwaggerConfiguration.AddSwaggerServices(services);

        // Assert
        var swaggerGenServices = services.Where(s =>
            s.ServiceType == typeof(IConfigureOptions<SwaggerGenOptions>)).ToList();
        Assert.NotEmpty(swaggerGenServices);
    }
}
