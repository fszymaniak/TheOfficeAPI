using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Level3.Controllers;
using TheOfficeAPI.Level3.Models;
using TheOfficeAPI.Level3.Services;

namespace TheOfficeAPI.Level3.Tests.Unit;

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
        var response = result?.Value as ApiResponseWithLinks<List<EpisodeWithLinks>>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("Episodes for season 1 retrieved successfully", response.Message);
    }

    [Fact]
    public void GetSeasonEpisodes_WithInvalidSeason_Returns404NotFound()
    {
        // Act
        var result = _controller.GetSeasonEpisodes(999) as NotFoundObjectResult;
        var response = result?.Value as ApiResponseWithLinks<object>;

        // Assert - Level 3 returns 404 Not Found for invalid resources
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Season parameter is outside of the scope", response.Error);
    }

    [Fact]
    public void GetSeasonEpisodes_IncludesHATEOASLinks()
    {
        // Act
        var result = _controller.GetSeasonEpisodes(1) as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<List<EpisodeWithLinks>>;

        // Assert - Response includes links
        Assert.NotNull(response?.Links);
        Assert.NotEmpty(response.Links);
        Assert.Contains(response.Links, link => link.Rel == "self");
        Assert.Contains(response.Links, link => link.Rel == "season");
        Assert.Contains(response.Links, link => link.Rel == "allSeasons");

        // Assert - Each episode includes links
        Assert.NotNull(response.Data);
        foreach (var episode in response.Data)
        {
            Assert.NotNull(episode.Links);
            Assert.NotEmpty(episode.Links);
            Assert.Contains(episode.Links, link => link.Rel == "self");
            Assert.Contains(episode.Links, link => link.Rel == "season");
            Assert.Contains(episode.Links, link => link.Rel == "allEpisodes");
        }
    }

    [Fact]
    public void GetSeasonEpisodes_EpisodesIncludeNextAndPreviousLinks()
    {
        // Act
        var result = _controller.GetSeasonEpisodes(1) as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<List<EpisodeWithLinks>>;

        // Assert
        Assert.NotNull(response?.Data);
        Assert.True(response.Data.Count >= 2);

        // First episode should not have previous, but should have next
        var firstEpisode = response.Data.First();
        Assert.DoesNotContain(firstEpisode.Links, link => link.Rel == "previous");
        Assert.Contains(firstEpisode.Links, link => link.Rel == "next");

        // Middle episodes should have both
        if (response.Data.Count > 2)
        {
            var middleEpisode = response.Data[1];
            Assert.Contains(middleEpisode.Links, link => link.Rel == "previous");
            Assert.Contains(middleEpisode.Links, link => link.Rel == "next");
        }

        // Last episode should have previous, but not next (within season)
        var lastEpisode = response.Data.Last();
        Assert.Contains(lastEpisode.Links, link => link.Rel == "previous");
        Assert.DoesNotContain(lastEpisode.Links, link => link.Rel == "next");
    }

    [Fact]
    public void GetEpisode_WithValidParameters_ReturnsSuccessResponse()
    {
        // Act
        var result = _controller.GetEpisode(1, 1) as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<EpisodeWithLinks>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("Episode retrieved successfully", response.Message);
    }

    [Fact]
    public void GetEpisode_WithInvalidSeason_Returns404NotFound()
    {
        // Act
        var result = _controller.GetEpisode(999, 1) as NotFoundObjectResult;
        var response = result?.Value as ApiResponseWithLinks<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Season parameter is outside of the scope", response.Error);
    }

    [Fact]
    public void GetEpisode_WithInvalidEpisode_Returns404NotFound()
    {
        // Act
        var result = _controller.GetEpisode(1, 999) as NotFoundObjectResult;
        var response = result?.Value as ApiResponseWithLinks<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Episode parameter is outside of the scope", response.Error);
    }

    [Fact]
    public void GetEpisode_IncludesHATEOASLinks()
    {
        // Act
        var result = _controller.GetEpisode(2, 1) as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<EpisodeWithLinks>;

        // Assert - Response includes links
        Assert.NotNull(response?.Links);
        Assert.NotEmpty(response.Links);
        Assert.Contains(response.Links, link => link.Rel == "self");
        Assert.Contains(response.Links, link => link.Rel == "allEpisodes");
        Assert.Contains(response.Links, link => link.Rel == "season");
        Assert.Contains(response.Links, link => link.Rel == "allSeasons");

        // Assert - Episode data includes links
        Assert.NotNull(response.Data?.Links);
        Assert.NotEmpty(response.Data.Links);
        Assert.Contains(response.Data.Links, link => link.Rel == "self");
        Assert.Contains(response.Data.Links, link => link.Rel == "season");
        Assert.Contains(response.Data.Links, link => link.Rel == "allEpisodes");
    }

    [Fact]
    public void GetEpisode_MiddleEpisode_IncludesNextAndPreviousLinks()
    {
        // Act - Season 2, Episode 2 should have both next and previous
        var result = _controller.GetEpisode(2, 2) as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<EpisodeWithLinks>;

        // Assert
        Assert.NotNull(response?.Data?.Links);
        Assert.Contains(response.Data.Links, link => link.Rel == "previous");
        Assert.Contains(response.Data.Links, link => link.Rel == "next");

        var previousLink = response.Data.Links.FirstOrDefault(l => l.Rel == "previous");
        Assert.NotNull(previousLink);
        Assert.Equal("/api/seasons/2/episodes/1", previousLink.Href);

        var nextLink = response.Data.Links.FirstOrDefault(l => l.Rel == "next");
        Assert.NotNull(nextLink);
        Assert.Equal("/api/seasons/2/episodes/3", nextLink.Href);
    }

    [Fact]
    public void GetEpisode_FirstEpisodeOfSeason_DoesNotIncludePreviousLink()
    {
        // Act
        var result = _controller.GetEpisode(1, 1) as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<EpisodeWithLinks>;

        // Assert
        Assert.NotNull(response?.Data?.Links);
        Assert.DoesNotContain(response.Data.Links, link => link.Rel == "previous");
        Assert.Contains(response.Data.Links, link => link.Rel == "next");
    }

    [Fact]
    public void GetEpisode_LastEpisodeOfSeason_IncludesNextSeasonLink()
    {
        // Act - Season 1, last episode (episode 6)
        var result = _controller.GetEpisode(1, 6) as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<EpisodeWithLinks>;

        // Assert - Should include link to next season's first episode
        Assert.NotNull(response?.Data?.Links);
        Assert.Contains(response.Data.Links, link => link.Rel == "nextSeason");

        var nextSeasonLink = response.Data.Links.FirstOrDefault(l => l.Rel == "nextSeason");
        Assert.NotNull(nextSeasonLink);
        Assert.Equal("/api/seasons/2/episodes/1", nextSeasonLink.Href);
    }

    [Fact]
    public void GetEpisode_LastEpisodeOfLastSeason_DoesNotIncludeNextSeasonLink()
    {
        // Act - Season 9, last episode (episode 23)
        var result = _controller.GetEpisode(9, 23) as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<EpisodeWithLinks>;

        // Assert - Should not include nextSeason link
        Assert.NotNull(response?.Data?.Links);
        Assert.DoesNotContain(response.Data.Links, link => link.Rel == "nextSeason");
    }

    [Fact]
    public void GetEpisode_NotFound_IncludesHelpfulLinks()
    {
        // Act
        var result = _controller.GetEpisode(1, 999) as NotFoundObjectResult;
        var response = result?.Value as ApiResponseWithLinks<object>;

        // Assert - Even error responses include helpful links
        Assert.NotNull(response?.Links);
        Assert.NotEmpty(response.Links);
        Assert.Contains(response.Links, link => link.Rel == "allEpisodes");
        Assert.Contains(response.Links, link => link.Rel == "season");
        Assert.Contains(response.Links, link => link.Rel == "allSeasons");
    }

    [Fact]
    public void GetEpisode_LinksHaveCorrectFormat()
    {
        // Act
        var result = _controller.GetEpisode(2, 1) as OkObjectResult;
        var response = result?.Value as ApiResponseWithLinks<EpisodeWithLinks>;

        // Assert
        Assert.NotNull(response?.Data?.Links);

        var selfLink = response.Data.Links.FirstOrDefault(l => l.Rel == "self");
        Assert.NotNull(selfLink);
        Assert.Equal("/api/seasons/2/episodes/1", selfLink.Href);
        Assert.Equal("GET", selfLink.Method);

        var seasonLink = response.Data.Links.FirstOrDefault(l => l.Rel == "season");
        Assert.NotNull(seasonLink);
        Assert.Equal("/api/seasons/2", seasonLink.Href);
        Assert.Equal("GET", seasonLink.Method);
    }
}
