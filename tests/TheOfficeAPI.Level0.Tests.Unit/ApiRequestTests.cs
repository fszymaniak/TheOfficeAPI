using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Level0.Tests.Unit;

public class ApiRequestTests
{
    [AllureXunit]
    public void ApiRequest_DefaultValues_AreCorrect()
    {
        // Act
        var request = new ApiRequest();

        // Assert
        Assert.Equal(string.Empty, request.Action);
        Assert.Null(request.Season);
        Assert.Null(request.Episode);
    }

    [AllureXunit]
    public void ApiRequest_PropertiesCanBeSet()
    {
        // Arrange
        var action = "getSeasonEpisodes";
        var season = 1;
        var episode = 5;

        // Act
        var request = new ApiRequest
        {
            Action = action,
            Season = season,
            Episode = episode
        };

        // Assert
        Assert.Equal(action, request.Action);
        Assert.Equal(season, request.Season);
        Assert.Equal(episode, request.Episode);
    }

    [Theory]
    [InlineData("getAllSeasons")]
    [InlineData("getSeasonEpisodes")]
    [InlineData("getEpisode")]
    [InlineData("")]
    public void ApiRequest_ActionProperty_AcceptsVariousValues(string action)
    {
        // Act
        var request = new ApiRequest { Action = action };

        // Assert
        Assert.Equal(action, request.Action);
    }

    [AllureXunit]
    public void ApiRequest_ActionProperty_AcceptsNullValue()
    {
        // Act
        var request = new ApiRequest { Action = null! };

        // Assert
        Assert.Null(request.Action);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(9)]
    [InlineData(-1)]
    [InlineData(0)]
    public void ApiRequest_SeasonProperty_AcceptsNullableIntValues(int? season)
    {
        // Act
        var request = new ApiRequest { Season = season };

        // Assert
        Assert.Equal(season, request.Season);
    }

    [AllureXunit]
    public void ApiRequest_SeasonProperty_AcceptsNullValue()
    {
        // Act
        var request = new ApiRequest { Season = null };

        // Assert
        Assert.Null(request.Season);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(22)]
    [InlineData(-1)]
    [InlineData(0)]
    public void ApiRequest_EpisodeProperty_AcceptsNullableIntValues(int? episode)
    {
        // Act
        var request = new ApiRequest { Episode = episode };

        // Assert
        Assert.Equal(episode, request.Episode);
    }

    [AllureXunit]
    public void ApiRequest_EpisodeProperty_AcceptsNullValue()
    {
        // Act
        var request = new ApiRequest { Episode = null };

        // Assert
        Assert.Null(request.Episode);
    }
}