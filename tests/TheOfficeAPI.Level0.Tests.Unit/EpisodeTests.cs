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

    [Fact]
    public void Episode_SeasonProperty_AcceptsNullValue()
    {
        // Act
        var episode = new Episode { Season = null };

        // Assert
        Assert.Null(episode.Season);
    }

    [Theory]
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

    [Fact]
    public void Episode_EpisodeNumberProperty_AcceptsNullValue()
    {
        // Act
        var episode = new Episode { EpisodeNumber = null };

        // Assert
        Assert.Null(episode.EpisodeNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Pilot")]
    [InlineData("The Dundies")]
    public void Episode_TitleProperty_AcceptsStringValues(string title)
    {
        // Act
        var episode = new Episode { Title = title };

        // Assert
        Assert.Equal(title, episode.Title);
    }

    [Fact]
    public void Episode_TitleProperty_AcceptsNullValue()
    {
        // Act
        var episode = new Episode { Title = null! };

        // Assert
        Assert.Null(episode.Title);
    }

    [Theory]
    [InlineData("")]
    [InlineData("2005-03-24")]
    [InlineData("March 24, 2005")]
    public void Episode_ReleasedDateProperty_AcceptsStringValues(string releasedDate)
    {
        // Act
        var episode = new Episode { ReleasedDate = releasedDate };

        // Assert
        Assert.Equal(releasedDate, episode.ReleasedDate);
    }

    [Fact]
    public void Episode_ReleasedDateProperty_AcceptsNullValue()
    {
        // Act
        var episode = new Episode { ReleasedDate = null! };

        // Assert
        Assert.Null(episode.ReleasedDate);
    }
}