using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Common.Models;
using TheOfficeAPI.Level0.Controllers;
using TheOfficeAPI.Level0.Services;

namespace TheOfficeAPI.Level0.Tests.Unit;

public class Level0ControllerTests
{
    private readonly Level0Controller _controller;
    private readonly TheOfficeService _service;

    public Level0ControllerTests()
    {
        _service = new TheOfficeService();
        _controller = new Level0Controller(_service);
    }

    [AllureXunit]
    public void HandleRequest_GetAllSeasons_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getAllSeasons" };

        // Act
        var result = _controller.HandleRequest(request) as OkObjectResult;
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
    public void HandleRequest_GetSeasonEpisodes_WithValidSeason_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getSeasonEpisodes", Season = 1 };

        // Act
        var result = _controller.HandleRequest(request) as OkObjectResult;
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
    public void HandleRequest_GetSeasonEpisodes_WithNullSeason_ReturnsErrorResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getSeasonEpisodes", Season = null };

        // Act
        var result = _controller.HandleRequest(request) as OkObjectResult;
        var response = result?.Value as ApiResponse<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("Season parameter is required", response.Error);
        Assert.Equal("Invalid request", response.Message);
    }

    [AllureXunit]
    public void HandleRequest_GetSeasonEpisodes_WithInvalidSeason_ReturnsErrorResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getSeasonEpisodes", Season = 999 };

        // Act
        var result = _controller.HandleRequest(request) as OkObjectResult;
        var response = result?.Value as ApiResponse<List<Episode>>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Season parameter is outside of the scope", response.Error);
        Assert.Equal("Invalid request", response.Message);
    }

    [AllureXunit]
    public void HandleRequest_GetEpisode_WithValidParameters_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getEpisode", Season = 1, Episode = 1 };

        // Act
        var result = _controller.HandleRequest(request) as OkObjectResult;
        var response = result?.Value as ApiResponse<Episode>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("Episode retrieved successfully", response.Message);
    }

    [Theory]
    [InlineData(null, 1)]
    [InlineData(1, null)]
    [InlineData(null, null)]
    public void HandleRequest_GetEpisode_WithNullParameters_ReturnsErrorResponse(int? season, int? episode)
    {
        // Arrange
        var request = new ApiRequest { Action = "getEpisode", Season = season, Episode = episode };

        // Act
        var result = _controller.HandleRequest(request) as OkObjectResult;
        var response = result?.Value as ApiResponse<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("Both season and episode parameters are required", response.Error);
        Assert.Equal("Invalid request", response.Message);
    }

    [AllureXunit]
    public void HandleRequest_GetEpisode_WithInvalidSeason_ReturnsErrorResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getEpisode", Season = 999, Episode = 1 };

        // Act
        var result = _controller.HandleRequest(request) as OkObjectResult;
        var response = result?.Value as ApiResponse<List<Episode>>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Season parameter is outside of the scope", response.Error);
        Assert.Equal("Invalid request", response.Message);
    }

    [AllureXunit]
    public void HandleRequest_GetEpisode_WithInvalidEpisode_ReturnsErrorResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getEpisode", Season = 1, Episode = 999 };

        // Act
        var result = _controller.HandleRequest(request) as OkObjectResult;
        var response = result?.Value as ApiResponse<List<Episode>>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Episode parameter is outside of the scope", response.Error);
        Assert.Equal("Invalid request", response.Message);
    }

    [AllureXunit]
    public void HandleRequest_UnknownAction_ReturnsErrorResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "unknownAction" };

        // Act
        var result = _controller.HandleRequest(request) as OkObjectResult;
        var response = result?.Value as ApiResponse<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("Unknown action: unknownAction", response.Error);
        Assert.Equal("Invalid action", response.Message);
    }

    [AllureXunit]
    public void HandleRequest_CaseInsensitiveActions_WorksCorrectly()
    {
        // Arrange
        var request = new ApiRequest { Action = "GETALLSEASONS" };

        // Act
        var result = _controller.HandleRequest(request) as OkObjectResult;
        var response = result?.Value as ApiResponse<List<Season>>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }
}