using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Common.Models;
using TheOfficeAPI.Level2.Controllers;
using TheOfficeAPI.Level2.Services;

namespace TheOfficeAPI.Level2.Tests.Unit;

public class SeasonsControllerTests
{
    private readonly SeasonsController _controller;
    private readonly TheOfficeService _service;

    public SeasonsControllerTests()
    {
        _service = new TheOfficeService();
        _controller = new SeasonsController(_service);
    }

    [AllureXunit]
    public void GetAllSeasons_ReturnsSuccessResponse()
    {
        // Act
        var result = _controller.GetAllSeasons() as OkObjectResult;
        var response = result?.Value as ApiResponse<List<Season>>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.NotEmpty(response.Data);
        Assert.Equal("Seasons retrieved successfully", response.Message);
    }

    [AllureXunit]
    public void GetAllSeasons_ReturnsAllSeasons()
    {
        // Act
        var result = _controller.GetAllSeasons() as OkObjectResult;
        var response = result?.Value as ApiResponse<List<Season>>;

        // Assert
        Assert.NotNull(response?.Data);
        Assert.Equal(9, response.Data.Count); // The Office has 9 seasons
    }

    [AllureXunit]
    public void GetSeason_WithValidSeasonNumber_ReturnsSuccessResponse()
    {
        // Act
        var result = _controller.GetSeason(1) as OkObjectResult;
        var response = result?.Value as ApiResponse<Season>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("1", response.Data.SeasonNumber);
        Assert.Equal("Season 1 retrieved successfully", response.Message);
    }

    [AllureXunit]
    public void GetSeason_WithInvalidSeasonNumber_Returns404NotFound()
    {
        // Act
        var result = _controller.GetSeason(999) as NotFoundObjectResult;
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
    public void GetSeason_WithSeasonZero_Returns404NotFound()
    {
        // Act
        var result = _controller.GetSeason(0) as NotFoundObjectResult;
        var response = result?.Value as ApiResponse<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Season parameter is outside of the scope", response.Error);
    }

    [AllureXunit]
    public void GetSeason_WithSeasonTwo_ReturnsCorrectSeason()
    {
        // Act
        var result = _controller.GetSeason(2) as OkObjectResult;
        var response = result?.Value as ApiResponse<Season>;

        // Assert
        Assert.NotNull(response?.Data);
        Assert.Equal("2", response.Data.SeasonNumber);
        Assert.Equal(22, response.Data.EpisodeCount); // Season 2 has 22 episodes
    }

    [AllureXunit]
    public void GetAllSeasons_ReturnsOkStatus()
    {
        // Act
        var result = _controller.GetAllSeasons() as OkObjectResult;

        // Assert - Level 2 uses proper HTTP status codes
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }
}
