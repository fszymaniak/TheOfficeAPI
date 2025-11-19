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
        var response = result?.Value as ApiResponseWithLinks<List<SeasonWithLinks>>;

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
        var response = result?.Value as ApiResponseWithLinks<List<SeasonWithLinks>>;

        // Assert
        Assert.NotNull(response?.Data);
        Assert.Equal(9, response.Data.Count); // The Office has 9 seasons
    }

    [Fact]
    public void GetAllSeasons_IncludesHATEOASLinks()
    {
        // Act
        var result = _controller.GetAllSeasons() as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<List<SeasonWithLinks>>;

        // Assert - Response includes links
        Assert.NotNull(response?.Links);
        Assert.NotEmpty(response.Links);
        Assert.Contains(response.Links, link => link.Rel == "self");

        // Assert - Each season includes links
        Assert.NotNull(response.Data);
        foreach (var season in response.Data)
        {
            Assert.NotNull(season.Links);
            Assert.NotEmpty(season.Links);
            Assert.Contains(season.Links, link => link.Rel == "self");
            Assert.Contains(season.Links, link => link.Rel == "episodes");
            Assert.Contains(season.Links, link => link.Rel == "allSeasons");
        }
    }

    [Fact]
    public void GetAllSeasons_LinksHaveCorrectFormat()
    {
        // Act
        var result = _controller.GetAllSeasons() as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<List<SeasonWithLinks>>;

        // Assert
        Assert.NotNull(response?.Data);
        var firstSeason = response.Data.First();

        var selfLink = firstSeason.Links.FirstOrDefault(l => l.Rel == "self");
        Assert.NotNull(selfLink);
        Assert.Equal($"/api/seasons/{firstSeason.SeasonNumber}", selfLink.Href);
        Assert.Equal("GET", selfLink.Method);

        var episodesLink = firstSeason.Links.FirstOrDefault(l => l.Rel == "episodes");
        Assert.NotNull(episodesLink);
        Assert.Equal($"/api/seasons/{firstSeason.SeasonNumber}/episodes", episodesLink.Href);
        Assert.Equal("GET", episodesLink.Method);
    }

    [Fact]
    public void GetSeason_WithValidSeasonNumber_ReturnsSuccessResponse()
    {
        // Act
        var result = _controller.GetSeason(1) as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<SeasonWithLinks>;

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
    public void GetSeason_WithInvalidSeasonNumber_Returns404NotFound()
    {
        // Act
        var result = _controller.GetSeason(999) as NotFoundObjectResult;
        var response = result?.Value as ApiResponseWithLinks<object>;

        // Assert - Level 3 returns 404 Not Found for invalid resources
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Season parameter is outside of the scope", response.Error);
    }

    [Fact]
    public void GetSeason_IncludesHATEOASLinks()
    {
        // Act
        var result = _controller.GetSeason(2) as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<SeasonWithLinks>;

        // Assert - Response includes links
        Assert.NotNull(response?.Links);
        Assert.NotEmpty(response.Links);
        Assert.Contains(response.Links, link => link.Rel == "self");
        Assert.Contains(response.Links, link => link.Rel == "allSeasons");

        // Assert - Season data includes links
        Assert.NotNull(response.Data?.Links);
        Assert.NotEmpty(response.Data.Links);
        Assert.Contains(response.Data.Links, link => link.Rel == "self");
        Assert.Contains(response.Data.Links, link => link.Rel == "episodes");
        Assert.Contains(response.Data.Links, link => link.Rel == "allSeasons");
    }

    [Fact]
    public void GetSeason_IncludesNextAndPreviousLinks()
    {
        // Act - Season 2 should have both next and previous
        var result = _controller.GetSeason(2) as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<SeasonWithLinks>;

        // Assert
        Assert.NotNull(response?.Data?.Links);
        Assert.Contains(response.Data.Links, link => link.Rel == "previous");
        Assert.Contains(response.Data.Links, link => link.Rel == "next");

        var previousLink = response.Data.Links.FirstOrDefault(l => l.Rel == "previous");
        Assert.NotNull(previousLink);
        Assert.Equal("/api/seasons/1", previousLink.Href);

        var nextLink = response.Data.Links.FirstOrDefault(l => l.Rel == "next");
        Assert.NotNull(nextLink);
        Assert.Equal("/api/seasons/3", nextLink.Href);
    }

    [Fact]
    public void GetSeason_FirstSeason_DoesNotIncludePreviousLink()
    {
        // Act
        var result = _controller.GetSeason(1) as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<SeasonWithLinks>;

        // Assert
        Assert.NotNull(response?.Data?.Links);
        Assert.DoesNotContain(response.Data.Links, link => link.Rel == "previous");
        Assert.Contains(response.Data.Links, link => link.Rel == "next");
    }

    [Fact]
    public void GetSeason_LastSeason_DoesNotIncludeNextLink()
    {
        // Act
        var result = _controller.GetSeason(9) as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<SeasonWithLinks>;

        // Assert
        Assert.NotNull(response?.Data?.Links);
        Assert.Contains(response.Data.Links, link => link.Rel == "previous");
        Assert.DoesNotContain(response.Data.Links, link => link.Rel == "next");
    }

    [Fact]
    public void GetSeason_NotFound_IncludesHelpfulLinks()
    {
        // Act
        var result = _controller.GetSeason(999) as NotFoundObjectResult;
        var response = result?.Value as ApiResponseWithLinks<object>;

        // Assert - Even error responses include helpful links
        Assert.NotNull(response?.Links);
        Assert.NotEmpty(response.Links);
        Assert.Contains(response.Links, link => link.Rel == "allSeasons");
    }
}
