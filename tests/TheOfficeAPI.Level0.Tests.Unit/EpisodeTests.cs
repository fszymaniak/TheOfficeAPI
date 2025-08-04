using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Level0.Tests.Unit;

public class EpisodeTests
{
    [Fact]
    public void Episode_DefaultValues_AreCorrect()
    {
        // Act
        var episode = new Episode();

        // Assert
        Assert.Null(episode.Season);
        Assert.Null(episode.EpisodeNumber);
        Assert.Equal(string.Empty, episode.Title);
        Assert.Equal(string.Empty, episode.ReleasedDate);
    }

    [Fact]
    public void Episode_PropertiesCanBeSet()
    {
        // Arrange
        var season = 1;
        var episodeNumber = 5;
        var title = "The Fight";
        var releasedDate = "2005-11-01";

        // Act
        var episode = new Episode
        {
            Season = season,
            EpisodeNumber = episodeNumber,
            Title = title,
            ReleasedDate = releasedDate
        };

        // Assert
        Assert.Equal(season, episode.Season);
        Assert.Equal(episodeNumber, episode.EpisodeNumber);
        Assert.Equal(title, episode.Title);
        Assert.Equal(releasedDate, episode.ReleasedDate);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(1)]
    [InlineData(9)]
    [InlineData(-1)]
    public void Episode_SeasonProperty_AcceptsNullableIntValues(int? season)
    {
        // Act
        var episode = new Episode { Season = season };

        // Assert
        Assert.Equal(season, episode.Season);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(1)]
    [InlineData(22)]
    [InlineData(0)]
    public void Episode_EpisodeNumberProperty_AcceptsNullableIntValues(int? episodeNumber)
    {
        // Act
        var episode = new Episode { EpisodeNumber = episodeNumber };

        // Assert
        Assert.Equal(episodeNumber, episode.EpisodeNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Pilot")]
    [InlineData("The Dundies")]
    [InlineData(null)]
    public void Episode_TitleProperty_AcceptsStringValues(string title)
    {
        // Act
        var episode = new Episode { Title = title ?? string.Empty };

        // Assert
        Assert.Equal(title ?? string.Empty, episode.Title);
    }

    [Theory]
    [InlineData("")]
    [InlineData("2005-03-24")]
    [InlineData("March 24, 2005")]
    [InlineData(null)]
    public void Episode_ReleasedDateProperty_AcceptsStringValues(string releasedDate)
    {
        // Act
        var episode = new Episode { ReleasedDate = releasedDate ?? string.Empty };

        // Assert
        Assert.Equal(releasedDate ?? string.Empty, episode.ReleasedDate);
    }
}