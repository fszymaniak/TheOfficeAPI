using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Common.Models;
using TheOfficeAPI.Level1.Controllers;
using TheOfficeAPI.Level1.Services;

namespace TheOfficeAPI.Level1.Tests.Unit;

public class EpisodesControllerTests
{
    private readonly EpisodesController _controller;
    private readonly TheOfficeService _service;

    public EpisodesControllerTests()
    {
        _service = new TheOfficeService();
        _controller = new EpisodesController(_service);
    }

    [Fact]
    public void GetSeasonEpisodes_WithValidSeason_ReturnsSuccessResponse()
    {
        // Act
        var result = _controller.GetSeasonEpisodes(1) as OkObjectResult;
        var response = result?.Value as ApiResponse<List<Episode>>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("Episodes for season 1 retrieved successfully", response.Message);
    }

    [Fact]
    public void GetSeasonEpisodes_WithInvalidSeason_ReturnsErrorResponse()
    {
        // Act
        var result = _controller.GetSeasonEpisodes(999) as OkObjectResult;
        var response = result?.Value as ApiResponse<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode); // Level 1 still returns 200 OK
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Season parameter is outside of the scope", response.Error);
        Assert.Equal("Invalid request", response.Message);
    }

    [Fact]
    public void GetSeasonEpisodes_WithSeasonZero_ReturnsErrorResponse()
    {
        // Act
        var result = _controller.GetSeasonEpisodes(0) as OkObjectResult;
        var response = result?.Value as ApiResponse<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Season parameter is outside of the scope", response.Error);
    }

    [Fact]
    public void GetEpisode_WithValidParameters_ReturnsSuccessResponse()
    {
        // Act
        var result = _controller.GetEpisode(1, 1) as OkObjectResult;
        var response = result?.Value as ApiResponse<Episode>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("Episode retrieved successfully", response.Message);
    }

    [Fact]
    public void GetEpisode_WithInvalidSeason_ReturnsErrorResponse()
    {
        // Act
        var result = _controller.GetEpisode(999, 1) as OkObjectResult;
        var response = result?.Value as ApiResponse<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Season parameter is outside of the scope", response.Error);
        Assert.Equal("Invalid request", response.Message);
    }

    [Fact]
    public void GetEpisode_WithInvalidEpisode_ReturnsErrorResponse()
    {
        // Act
        var result = _controller.GetEpisode(1, 999) as OkObjectResult;
        var response = result?.Value as ApiResponse<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Episode parameter is outside of the scope", response.Error);
        Assert.Equal("Invalid request", response.Message);
    }

    [Fact]
    public void GetEpisode_WithValidSeasonAndEpisode_ReturnsCorrectEpisode()
    {
        // Act
        var result = _controller.GetEpisode(2, 1) as OkObjectResult;
        var response = result?.Value as ApiResponse<Episode>;

        // Assert
        Assert.NotNull(response?.Data);
        Assert.Equal(2, response.Data.Season);
        Assert.Equal(1, response.Data.EpisodeNumber);
    }

    [Fact]
    public void GetSeasonEpisodes_ReturnsCorrectNumberOfEpisodes()
    {
        // Act
        var result = _controller.GetSeasonEpisodes(1) as OkObjectResult;
        var response = result?.Value as ApiResponse<List<Episode>>;

        // Assert
        Assert.NotNull(response?.Data);
        Assert.Equal(6, response.Data.Count); // Season 1 has 6 episodes
    }
}
