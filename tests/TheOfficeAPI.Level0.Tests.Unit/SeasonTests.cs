using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Level0.Tests.Unit;

public class SeasonTests
{
    [Fact]
    public void Season_DefaultValues_AreCorrect()
    {
        // Act
        var season = new Season();

        // Assert
        Assert.Equal(string.Empty, season.SeasonNumber);
        Assert.Equal(0, season.EpisodeCount);
    }

    [Fact]
    public void Season_PropertiesCanBeSet()
    {
        // Arrange
        var seasonNumber = "1";
        var episodeCount = 6;

        // Act
        var season = new Season
        {
            SeasonNumber = seasonNumber,
            EpisodeCount = episodeCount
        };

        // Assert
        Assert.Equal(seasonNumber, season.SeasonNumber);
        Assert.Equal(episodeCount, season.EpisodeCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("9")]
    [InlineData("10")]
    [InlineData(null)]
    public void Season_SeasonNumberProperty_AcceptsStringValues(string seasonNumber)
    {
        // Act
        var season = new Season { SeasonNumber = seasonNumber ?? string.Empty };

        // Assert
        Assert.Equal(seasonNumber ?? string.Empty, season.SeasonNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(22)]
    [InlineData(-1)]
    public void Season_EpisodeCountProperty_AcceptsIntValues(int episodeCount)
    {
        // Act
        var season = new Season { EpisodeCount = episodeCount };

        // Assert
        Assert.Equal(episodeCount, season.EpisodeCount);
    }

    [Fact]
    public void Season_WithValidData_RepresentsSeasonCorrectly()
    {
        // Arrange & Act
        var season = new Season
        {
            SeasonNumber = "2",
            EpisodeCount = 22
        };

        // Assert
        Assert.Equal("2", season.SeasonNumber);
        Assert.Equal(22, season.EpisodeCount);
    }
}