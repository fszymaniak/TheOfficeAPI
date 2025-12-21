using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Level3.Controllers;
using TheOfficeAPI.Level3.Models;
using TheOfficeAPI.Level3.Services;

namespace TheOfficeAPI.Level3.Tests.Unit;

public class SeasonsControllerTests
{
    private readonly SeasonsController _controller;
    private readonly TheOfficeService _service;

    public SeasonsControllerTests()
    {
        _service = new TheOfficeService();
        _controller = new SeasonsController(_service);
    }

    [Fact]
    public void GetAllSeasons_ReturnsSuccessResponse()
    {
        // Act
        var result = _controller.GetAllSeasons() as OkObjectResult;
        var response = result?.Value as HateoasResponse<List<SeasonResource>>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.NotEmpty(response.Data);
        Assert.Equal("Seasons retrieved successfully", response.Message);
    }

    [Fact]
    public void GetAllSeasons_ReturnsAllSeasons()
    {
        // Act
        var result = _controller.GetAllSeasons() as OkObjectResult;
        var response = result?.Value as HateoasResponse<List<SeasonResource>>;

        // Assert
        Assert.NotNull(response?.Data);
        Assert.Equal(9, response.Data.Count); // The Office has 9 seasons
    }

    [Fact]
    public void GetAllSeasons_IncludesHypermediaLinks()
    {
        // Act
        var result = _controller.GetAllSeasons() as OkObjectResult;
        var response = result?.Value as HateoasResponse<List<SeasonResource>>;

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Links);
        Assert.NotEmpty(response.Links);

        // Check response-level self link
        var selfLink = response.Links.FirstOrDefault(l => l.Rel == "self");
        Assert.NotNull(selfLink);
        Assert.Equal("/api/v3/seasons", selfLink.Href);
        Assert.Equal("GET", selfLink.Method);

        // Check each season has links
        Assert.All(response.Data!, season =>
        {
            Assert.NotNull(season.Links);
            Assert.NotEmpty(season.Links);

            var seasonSelfLink = season.Links.FirstOrDefault(l => l.Rel == "self");
            Assert.NotNull(seasonSelfLink);

            var episodesLink = season.Links.FirstOrDefault(l => l.Rel == "episodes");
            Assert.NotNull(episodesLink);
        });
    }

    [Fact]
    public void GetSeason_WithValidSeasonNumber_ReturnsSuccessResponse()
    {
        // Act
        var result = _controller.GetSeason(1) as OkObjectResult;
        var response = result?.Value as HateoasResponse<SeasonResource>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("1", response.Data.SeasonNumber);
        Assert.Equal("Season 1 retrieved successfully", response.Message);
    }

    [Fact]
    public void GetSeason_IncludesNavigationLinks()
    {
        // Act
        var result = _controller.GetSeason(2) as OkObjectResult;
        var response = result?.Value as HateoasResponse<SeasonResource>;

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.Links);
        Assert.NotEmpty(response.Data.Links);

        // Check for self link
        var selfLink = response.Data.Links.FirstOrDefault(l => l.Rel == "self");
        Assert.NotNull(selfLink);
        Assert.Equal("/api/v3/seasons/2", selfLink.Href);

        // Check for episodes link
        var episodesLink = response.Data.Links.FirstOrDefault(l => l.Rel == "episodes");
        Assert.NotNull(episodesLink);
        Assert.Equal("/api/v3/seasons/2/episodes", episodesLink.Href);

        // Check for collection link
        var collectionLink = response.Data.Links.FirstOrDefault(l => l.Rel == "collection");
        Assert.NotNull(collectionLink);
        Assert.Equal("/api/v3/seasons", collectionLink.Href);
    }

    [Fact]
    public void GetSeason_WithInvalidSeasonNumber_Returns404NotFound()
    {
        // Act
        var result = _controller.GetSeason(999) as NotFoundObjectResult;
        var response = result?.Value as HateoasResponse<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Season parameter is outside of the scope", response.Error);
        Assert.Equal("Invalid request", response.Message);

        // Even error responses should include helpful links
        Assert.NotNull(response.Links);
        var collectionLink = response.Links.FirstOrDefault(l => l.Rel == "collection");
        Assert.NotNull(collectionLink);
    }

    [Fact]
    public void GetSeason_WithSeasonZero_Returns404NotFound()
    {
        // Act
        var result = _controller.GetSeason(0) as NotFoundObjectResult;
        var response = result?.Value as HateoasResponse<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Season parameter is outside of the scope", response.Error);
    }

    [Fact]
    public void GetSeason_WithSeasonTwo_ReturnsCorrectSeason()
    {
        // Act
        var result = _controller.GetSeason(2) as OkObjectResult;
        var response = result?.Value as HateoasResponse<SeasonResource>;

        // Assert
        Assert.NotNull(response?.Data);
        Assert.Equal("2", response.Data.SeasonNumber);
        Assert.True(response.Data.EpisodeCount > 0);
    }
}
