using TheOfficeAPI.Configuration;

namespace TheOfficeAPI.Common.Tests.Unit;

public class ConfigurationOptionsTests
{
    [AllureXunit]
    public void EnvironmentOptions_SectionName_ReturnsEnvironment()
    {
        // Assert
        Assert.Equal("Environment", EnvironmentOptions.SectionName);
    }

    [AllureXunit]
    public void EnvironmentOptions_MaturityLevelVariable_DefaultsToEmptyString()
    {
        // Arrange
        var options = new EnvironmentOptions();

        // Assert
        Assert.Equal(string.Empty, options.MaturityLevelVariable);
    }

    [AllureXunit]
    public void EnvironmentOptions_MaturityLevelVariable_CanBeSet()
    {
        // Arrange
        var options = new EnvironmentOptions();

        // Act
        options.MaturityLevelVariable = "CUSTOM_MATURITY_LEVEL";

        // Assert
        Assert.Equal("CUSTOM_MATURITY_LEVEL", options.MaturityLevelVariable);
    }

    [AllureXunit]
    public void ServerOptions_SectionName_ReturnsServer()
    {
        // Assert
        Assert.Equal("Server", ServerOptions.SectionName);
    }

    [AllureXunit]
    public void ServerOptions_DefaultUrl_DefaultsToEmptyString()
    {
        // Arrange
        var options = new ServerOptions();

        // Assert
        Assert.Equal(string.Empty, options.DefaultUrl);
    }

    [AllureXunit]
    public void ServerOptions_DefaultUrl_CanBeSet()
    {
        // Arrange
        var options = new ServerOptions();

        // Act
        options.DefaultUrl = "http://localhost:5000";

        // Assert
        Assert.Equal("http://localhost:5000", options.DefaultUrl);
    }

    [AllureXunit]
    public void EnvironmentOptions_IsInstantiable()
    {
        // Act
        var options = new EnvironmentOptions();

        // Assert
        Assert.NotNull(options);
    }

    [AllureXunit]
    public void ServerOptions_IsInstantiable()
    {
        // Act
        var options = new ServerOptions();

        // Assert
        Assert.NotNull(options);
    }
}
