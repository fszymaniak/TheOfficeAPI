using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Common.Models;
using TheOfficeAPI.Level1.Controllers;
using TheOfficeAPI.Level1.Services;

namespace TheOfficeAPI.Level1.Tests.Unit;

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
    public void GetAllSeasons_AlwaysReturnsOkStatus()
    {
        // Act
        var result = _controller.GetAllSeasons() as OkObjectResult;

        // Assert - Level 1 still returns 200 OK for all responses
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }
}
