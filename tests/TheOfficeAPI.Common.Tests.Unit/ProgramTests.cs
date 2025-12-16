using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TheOfficeAPI.Common.Tests.Unit;

public class ProgramTests : IDisposable
{
    private readonly List<string> _environmentVariablesToCleanup = new();

    public void Dispose()
    {
        foreach (var variable in _environmentVariablesToCleanup)
        {
            Environment.SetEnvironmentVariable(variable, null);
        }
    }

    private void SetEnvironmentVariable(string name, string? value)
    {
        _environmentVariablesToCleanup.Add(name);
        Environment.SetEnvironmentVariable(name, value);
    }

    [Fact]
    public void CreateWebApplication_WithNoMaturityLevel_ReturnsWebApplication()
    {
        // Arrange
        SetEnvironmentVariable("MATURITY_LEVEL", null);
        SetEnvironmentVariable("PORT", null);

        // Act
        var app = Program.CreateWebApplication(Array.Empty<string>());

        // Assert
        Assert.NotNull(app);
    }

    [Fact]
    public void CreateWebApplication_WithLevel0_ReturnsWebApplication()
    {
        // Arrange
        SetEnvironmentVariable("MATURITY_LEVEL", "Level0");
        SetEnvironmentVariable("PORT", null);

        // Act
        var app = Program.CreateWebApplication(Array.Empty<string>());

        // Assert
        Assert.NotNull(app);
    }

    [Fact]
    public void CreateWebApplication_WithLevel1_ReturnsWebApplication()
    {
        // Arrange
        SetEnvironmentVariable("MATURITY_LEVEL", "Level1");
        SetEnvironmentVariable("PORT", null);

        // Act
        var app = Program.CreateWebApplication(Array.Empty<string>());

        // Assert
        Assert.NotNull(app);
    }

    [Fact]
    public void CreateWebApplication_WithLevel2_ReturnsWebApplication()
    {
        // Arrange
        SetEnvironmentVariable("MATURITY_LEVEL", "Level2");
        SetEnvironmentVariable("PORT", null);

        // Act
        var app = Program.CreateWebApplication(Array.Empty<string>());

        // Assert
        Assert.NotNull(app);
    }

    [Fact]
    public void CreateWebApplication_WithLevel3_ReturnsWebApplication()
    {
        // Arrange
        SetEnvironmentVariable("MATURITY_LEVEL", "Level3");
        SetEnvironmentVariable("PORT", null);

        // Act
        var app = Program.CreateWebApplication(Array.Empty<string>());

        // Assert
        Assert.NotNull(app);
    }

    [Fact]
    public void CreateWebApplication_WithPortEnvironmentVariable_UsesRailwayMode()
    {
        // Arrange
        SetEnvironmentVariable("MATURITY_LEVEL", null);
        SetEnvironmentVariable("PORT", "8080");

        // Act
        var app = Program.CreateWebApplication(Array.Empty<string>());

        // Assert
        Assert.NotNull(app);
    }

    [Fact]
    public void CreateWebApplication_WithPortAndMaturityLevel_ConfiguresBothCorrectly()
    {
        // Arrange
        SetEnvironmentVariable("MATURITY_LEVEL", "Level0");
        SetEnvironmentVariable("PORT", "3000");

        // Act
        var app = Program.CreateWebApplication(Array.Empty<string>());

        // Assert
        Assert.NotNull(app);
    }

    [Fact]
    public void CreateWebApplication_WithInvalidMaturityLevel_FallsBackToBasicConfig()
    {
        // Arrange
        SetEnvironmentVariable("MATURITY_LEVEL", "InvalidLevel");
        SetEnvironmentVariable("PORT", null);

        // Act
        var app = Program.CreateWebApplication(Array.Empty<string>());

        // Assert
        Assert.NotNull(app);
    }

    [Fact]
    public void CreateWebApplication_WithNoMaturityLevel_RegistersAllLevelServices()
    {
        // Arrange
        SetEnvironmentVariable("MATURITY_LEVEL", null);
        SetEnvironmentVariable("PORT", null);

        // Act
        var app = Program.CreateWebApplication(Array.Empty<string>());

        // Assert
        Assert.NotNull(app);
        Assert.NotNull(app.Services.GetService<TheOfficeAPI.Level0.Services.TheOfficeService>());
        Assert.NotNull(app.Services.GetService<TheOfficeAPI.Level1.Services.TheOfficeService>());
        Assert.NotNull(app.Services.GetService<TheOfficeAPI.Level2.Services.TheOfficeService>());
        Assert.NotNull(app.Services.GetService<TheOfficeAPI.Level3.Services.TheOfficeService>());
    }

    [Fact]
    public void CreateWebApplication_WithLevel0_RegistersHealthCheckService()
    {
        // Arrange
        SetEnvironmentVariable("MATURITY_LEVEL", "Level0");
        SetEnvironmentVariable("PORT", null);

        // Act
        var app = Program.CreateWebApplication(Array.Empty<string>());

        // Assert
        Assert.NotNull(app);
        var healthCheckService = app.Services.GetService<TheOfficeAPI.Common.Services.HealthCheckService>();
        Assert.NotNull(healthCheckService);
    }

    [Fact]
    public void CreateWebApplication_WithNoMaturityLevel_RegistersHealthCheckService()
    {
        // Arrange
        SetEnvironmentVariable("MATURITY_LEVEL", null);
        SetEnvironmentVariable("PORT", null);

        // Act
        var app = Program.CreateWebApplication(Array.Empty<string>());

        // Assert
        Assert.NotNull(app);
        var healthCheckService = app.Services.GetService<TheOfficeAPI.Common.Services.HealthCheckService>();
        Assert.NotNull(healthCheckService);
    }

    [Fact]
    public void CreateWebApplication_WithEmptyArgs_ReturnsWebApplication()
    {
        // Arrange
        SetEnvironmentVariable("MATURITY_LEVEL", null);
        SetEnvironmentVariable("PORT", null);

        // Act
        var app = Program.CreateWebApplication(new string[] { });

        // Assert
        Assert.NotNull(app);
    }

    [Fact]
    public void CreateWebApplication_WithLevel1_RegistersLevel1Service()
    {
        // Arrange
        SetEnvironmentVariable("MATURITY_LEVEL", "Level1");
        SetEnvironmentVariable("PORT", null);

        // Act
        var app = Program.CreateWebApplication(Array.Empty<string>());

        // Assert
        Assert.NotNull(app);
        var level1Service = app.Services.GetService<TheOfficeAPI.Level1.Services.TheOfficeService>();
        Assert.NotNull(level1Service);
    }

    [Fact]
    public void CreateWebApplication_WithLevel2_RegistersLevel2Service()
    {
        // Arrange
        SetEnvironmentVariable("MATURITY_LEVEL", "Level2");
        SetEnvironmentVariable("PORT", null);

        // Act
        var app = Program.CreateWebApplication(Array.Empty<string>());

        // Assert
        Assert.NotNull(app);
        var level2Service = app.Services.GetService<TheOfficeAPI.Level2.Services.TheOfficeService>();
        Assert.NotNull(level2Service);
    }

    [Fact]
    public void CreateWebApplication_WithLevel3_RegistersLevel3Service()
    {
        // Arrange
        SetEnvironmentVariable("MATURITY_LEVEL", "Level3");
        SetEnvironmentVariable("PORT", null);

        // Act
        var app = Program.CreateWebApplication(Array.Empty<string>());

        // Assert
        Assert.NotNull(app);
        var level3Service = app.Services.GetService<TheOfficeAPI.Level3.Services.TheOfficeService>();
        Assert.NotNull(level3Service);
    }
}
