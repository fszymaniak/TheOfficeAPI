using Microsoft.Extensions.DependencyInjection;
using TheOfficeAPI.Level0.Extensions;
using TheOfficeAPI.Level1.Extensions;
using TheOfficeAPI.Level2.Extensions;
using TheOfficeAPI.Level3.Extensions;

namespace TheOfficeAPI.Common.Tests.Unit;

public class LevelServiceExtensionsTests
{
    [AllureXunit]
    public void AddLevel0Services_RegistersTheOfficeService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLevel0Services();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<TheOfficeAPI.Level0.Services.TheOfficeService>();
        Assert.NotNull(service);
    }

    [AllureXunit]
    public void AddLevel0Services_ReturnsServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddLevel0Services();

        // Assert
        Assert.Same(services, result);
    }

    [AllureXunit]
    public void AddLevel1Services_RegistersTheOfficeService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLevel1Services();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<TheOfficeAPI.Level1.Services.TheOfficeService>();
        Assert.NotNull(service);
    }

    [AllureXunit]
    public void AddLevel1Services_ReturnsServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddLevel1Services();

        // Assert
        Assert.Same(services, result);
    }

    [AllureXunit]
    public void AddLevel2Services_RegistersTheOfficeService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLevel2Services();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<TheOfficeAPI.Level2.Services.TheOfficeService>();
        Assert.NotNull(service);
    }

    [AllureXunit]
    public void AddLevel2Services_ReturnsServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddLevel2Services();

        // Assert
        Assert.Same(services, result);
    }

    [AllureXunit]
    public void AddLevel3Services_RegistersTheOfficeService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLevel3Services();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<TheOfficeAPI.Level3.Services.TheOfficeService>();
        Assert.NotNull(service);
    }

    [AllureXunit]
    public void AddLevel3Services_ReturnsServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddLevel3Services();

        // Assert
        Assert.Same(services, result);
    }

    [AllureXunit]
    public void AllLevelServices_CanBeRegisteredTogether()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLevel0Services()
                .AddLevel1Services()
                .AddLevel2Services()
                .AddLevel3Services();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var service0 = serviceProvider.GetService<TheOfficeAPI.Level0.Services.TheOfficeService>();
        var service1 = serviceProvider.GetService<TheOfficeAPI.Level1.Services.TheOfficeService>();
        var service2 = serviceProvider.GetService<TheOfficeAPI.Level2.Services.TheOfficeService>();
        var service3 = serviceProvider.GetService<TheOfficeAPI.Level3.Services.TheOfficeService>();

        Assert.NotNull(service0);
        Assert.NotNull(service1);
        Assert.NotNull(service2);
        Assert.NotNull(service3);
    }

    [AllureXunit]
    public void Level0Service_IsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLevel0Services();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var service1 = serviceProvider.GetService<TheOfficeAPI.Level0.Services.TheOfficeService>();
        var service2 = serviceProvider.GetService<TheOfficeAPI.Level0.Services.TheOfficeService>();

        // Assert
        Assert.Same(service1, service2);
    }

    [AllureXunit]
    public void Level1Service_IsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLevel1Services();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var service1 = serviceProvider.GetService<TheOfficeAPI.Level1.Services.TheOfficeService>();
        var service2 = serviceProvider.GetService<TheOfficeAPI.Level1.Services.TheOfficeService>();

        // Assert
        Assert.Same(service1, service2);
    }

    [AllureXunit]
    public void Level2Service_IsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLevel2Services();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var service1 = serviceProvider.GetService<TheOfficeAPI.Level2.Services.TheOfficeService>();
        var service2 = serviceProvider.GetService<TheOfficeAPI.Level2.Services.TheOfficeService>();

        // Assert
        Assert.Same(service1, service2);
    }

    [AllureXunit]
    public void Level3Service_IsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLevel3Services();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var service1 = serviceProvider.GetService<TheOfficeAPI.Level3.Services.TheOfficeService>();
        var service2 = serviceProvider.GetService<TheOfficeAPI.Level3.Services.TheOfficeService>();

        // Assert
        Assert.Same(service1, service2);
    }
}
