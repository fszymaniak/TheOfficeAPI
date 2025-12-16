using TheOfficeAPI.Configuration;

namespace TheOfficeAPI.Common.Tests.Unit;

public class ConfigurationOptionsTests
{
    [Fact]
    public void EnvironmentOptions_SectionName_ReturnsEnvironment()
    {
        // Assert
        Assert.Equal("Environment", EnvironmentOptions.SectionName);
    }

    [Fact]
    public void EnvironmentOptions_MaturityLevelVariable_DefaultsToEmptyString()
    {
        // Arrange
        var options = new EnvironmentOptions();

        // Assert
        Assert.Equal(string.Empty, options.MaturityLevelVariable);
    }

    [Fact]
    public void EnvironmentOptions_MaturityLevelVariable_CanBeSet()
    {
        // Arrange
        var options = new EnvironmentOptions();

        // Act
        options.MaturityLevelVariable = "CUSTOM_MATURITY_LEVEL";

        // Assert
        Assert.Equal("CUSTOM_MATURITY_LEVEL", options.MaturityLevelVariable);
    }

    [Fact]
    public void ServerOptions_SectionName_ReturnsServer()
    {
        // Assert
        Assert.Equal("Server", ServerOptions.SectionName);
    }

    [Fact]
    public void ServerOptions_DefaultUrl_DefaultsToEmptyString()
    {
        // Arrange
        var options = new ServerOptions();

        // Assert
        Assert.Equal(string.Empty, options.DefaultUrl);
    }

    [Fact]
    public void ServerOptions_DefaultUrl_CanBeSet()
    {
        // Arrange
        var options = new ServerOptions();

        // Act
        options.DefaultUrl = "http://localhost:5000";

        // Assert
        Assert.Equal("http://localhost:5000", options.DefaultUrl);
    }

    [Fact]
    public void EnvironmentOptions_IsInstantiable()
    {
        // Act
        var options = new EnvironmentOptions();

        // Assert
        Assert.NotNull(options);
    }

    [Fact]
    public void ServerOptions_IsInstantiable()
    {
        // Act
        var options = new ServerOptions();

        // Assert
        Assert.NotNull(options);
    }
}
