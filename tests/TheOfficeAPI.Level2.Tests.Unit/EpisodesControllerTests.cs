using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Common.Models;
using TheOfficeAPI.Level2.Controllers;
using TheOfficeAPI.Level2.Services;

namespace TheOfficeAPI.Level2.Tests.Unit;

public class EpisodesControllerTests
{
    private readonly EpisodesController _controller;
    private readonly TheOfficeService _service;

    public EpisodesControllerTests()
    {
        _service = new TheOfficeService();
        _controller = new EpisodesController(_service);
    }

    [AllureXunit]
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

    [AllureXunit]
    public void GetSeasonEpisodes_WithInvalidSeason_Returns404NotFound()
    {
        // Act
        var result = _controller.GetSeasonEpisodes(999) as NotFoundObjectResult;
        var response = result?.Value as ApiResponse<object>;

        // Assert - Level 2 returns 404 Not Found for invalid resources
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Season parameter is outside of the scope", response.Error);
        Assert.Equal("Invalid request", response.Message);
    }

    [AllureXunit]
    public void GetSeasonEpisodes_WithSeasonZero_Returns404NotFound()
    {
        // Act
        var result = _controller.GetSeasonEpisodes(0) as NotFoundObjectResult;
        var response = result?.Value as ApiResponse<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Season parameter is outside of the scope", response.Error);
    }

    [AllureXunit]
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

    [AllureXunit]
    public void GetEpisode_WithInvalidSeason_Returns404NotFound()
    {
        // Act
        var result = _controller.GetEpisode(999, 1) as NotFoundObjectResult;
        var response = result?.Value as ApiResponse<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Season parameter is outside of the scope", response.Error);
        Assert.Equal("Invalid request", response.Message);
    }

    [AllureXunit]
    public void GetEpisode_WithInvalidEpisode_Returns404NotFound()
    {
        // Act
        var result = _controller.GetEpisode(1, 999) as NotFoundObjectResult;
        var response = result?.Value as ApiResponse<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Episode parameter is outside of the scope", response.Error);
        Assert.Equal("Invalid request", response.Message);
    }

    [AllureXunit]
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

    [AllureXunit]
    public void GetSeasonEpisodes_ReturnsCorrectNumberOfEpisodes()
    {
        // Act
        var result = _controller.GetSeasonEpisodes(1) as OkObjectResult;
        var response = result?.Value as ApiResponse<List<Episode>>;

        // Assert
        Assert.NotNull(response?.Data);
        Assert.Equal(6, response.Data.Count); // Season 1 has 6 episodes
    }

    [AllureXunit]
    public void GetSeasonEpisodes_ForSeasonTwo_ReturnsCorrectNumberOfEpisodes()
    {
        // Act
        var result = _controller.GetSeasonEpisodes(2) as OkObjectResult;
        var response = result?.Value as ApiResponse<List<Episode>>;

        // Assert
        Assert.NotNull(response?.Data);
        Assert.Equal(22, response.Data.Count); // Season 2 has 22 episodes
    }

    [AllureXunit]
    public void GetEpisode_ReturnsProperHttpStatusCodes()
    {
        // Act - Valid request
        var validResult = _controller.GetEpisode(1, 1) as OkObjectResult;

        // Act - Invalid request
        var invalidResult = _controller.GetEpisode(999, 1) as NotFoundObjectResult;

        // Assert - Level 2 uses proper HTTP status codes
        Assert.Equal(200, validResult?.StatusCode);
        Assert.Equal(404, invalidResult?.StatusCode);
    }
}
