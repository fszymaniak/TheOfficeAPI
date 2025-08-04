using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Level0.Tests.Unit;

public class ApiRequestTests
{
    [Fact]
    public void ApiRequest_DefaultValues_AreCorrect()
    {
        // Act
        var request = new ApiRequest();

        // Assert
        Assert.Equal(string.Empty, request.Action);
        Assert.Null(request.Season);
        Assert.Null(request.Episode);
    }

    [Fact]
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
    [InlineData(null)]
    public void ApiRequest_ActionProperty_AcceptsVariousValues(string action)
    {
        // Act
        var request = new ApiRequest { Action = action ?? string.Empty };

        // Assert
        Assert.Equal(action ?? string.Empty, request.Action);
    }

    [Theory]
    [InlineData(null)]
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

    [Theory]
    [InlineData(null)]
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
}