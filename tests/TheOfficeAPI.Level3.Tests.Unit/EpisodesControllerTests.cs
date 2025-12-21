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

    [AllureXunit]
    public void GetSeasonEpisodes_WithValidSeason_ReturnsSuccessResponse()
    {
        // Act
        var result = _controller.GetSeasonEpisodes(1) as OkObjectResult;
        var response = result?.Value as HateoasResponse<List<EpisodeResource>>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.NotEmpty(response.Data);
        Assert.Equal("Episodes for season 1 retrieved successfully", response.Message);
    }

    [AllureXunit]
    public void GetSeasonEpisodes_IncludesHypermediaLinks()
    {
        // Act
        var result = _controller.GetSeasonEpisodes(1) as OkObjectResult;
        var response = result?.Value as HateoasResponse<List<EpisodeResource>>;

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Links);
        Assert.NotEmpty(response.Links);

        // Check response-level links
        var selfLink = response.Links.FirstOrDefault(l => l.Rel == "self");
        Assert.NotNull(selfLink);

        var seasonLink = response.Links.FirstOrDefault(l => l.Rel == "season");
        Assert.NotNull(seasonLink);

        var collectionLink = response.Links.FirstOrDefault(l => l.Rel == "collection");
        Assert.NotNull(collectionLink);

        // Check each episode has links
        Assert.All(response.Data!, episode =>
        {
            Assert.NotNull(episode.Links);
            Assert.NotEmpty(episode.Links);
        });
    }

    [AllureXunit]
    public void GetSeasonEpisodes_WithInvalidSeason_Returns404NotFound()
    {
        // Act
        var result = _controller.GetSeasonEpisodes(999) as NotFoundObjectResult;
        var response = result?.Value as HateoasResponse<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Season parameter is outside of the scope", response.Error);
        Assert.Equal("Invalid request", response.Message);
    }

    [AllureXunit]
    public void GetEpisode_WithValidParameters_ReturnsSuccessResponse()
    {
        // Act
        var result = _controller.GetEpisode(1, 1) as OkObjectResult;
        var response = result?.Value as HateoasResponse<EpisodeResource>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(1, response.Data.Season);
        Assert.Equal(1, response.Data.EpisodeNumber);
        Assert.Equal("Episode retrieved successfully", response.Message);
    }

    [AllureXunit]
    public void GetEpisode_IncludesNextAndPreviousLinks()
    {
        // Act - Get middle episode
        var result = _controller.GetEpisode(2, 5) as OkObjectResult;
        var response = result?.Value as HateoasResponse<EpisodeResource>;

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.Links);

        // Check for next link
        var nextLink = response.Data.Links.FirstOrDefault(l => l.Rel == "next");
        Assert.NotNull(nextLink);
        Assert.Equal("/api/v3/seasons/2/episodes/6", nextLink.Href);

        // Check for previous link
        var previousLink = response.Data.Links.FirstOrDefault(l => l.Rel == "previous");
        Assert.NotNull(previousLink);
        Assert.Equal("/api/v3/seasons/2/episodes/4", previousLink.Href);

        // Check for parent links
        var seasonLink = response.Data.Links.FirstOrDefault(l => l.Rel == "season");
        Assert.NotNull(seasonLink);

        var episodesLink = response.Data.Links.FirstOrDefault(l => l.Rel == "episodes");
        Assert.NotNull(episodesLink);

        var collectionLink = response.Data.Links.FirstOrDefault(l => l.Rel == "collection");
        Assert.NotNull(collectionLink);
    }

    [AllureXunit]
    public void GetFirstEpisode_HasNoPreviousLink()
    {
        // Act - Get first episode
        var result = _controller.GetEpisode(1, 1) as OkObjectResult;
        var response = result?.Value as HateoasResponse<EpisodeResource>;

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.Links);

        // Check that previous link does not exist
        var previousLink = response.Data.Links.FirstOrDefault(l => l.Rel == "previous");
        Assert.Null(previousLink);

        // But next link should exist
        var nextLink = response.Data.Links.FirstOrDefault(l => l.Rel == "next");
        Assert.NotNull(nextLink);
    }

    [AllureXunit]
    public void GetEpisode_WithInvalidSeason_Returns404NotFound()
    {
        // Act
        var result = _controller.GetEpisode(999, 1) as NotFoundObjectResult;
        var response = result?.Value as HateoasResponse<object>;

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
        var response = result?.Value as HateoasResponse<object>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("Episode parameter is outside of the scope", response.Error);
        Assert.Equal("Invalid request", response.Message);
    }

    [AllureXunit]
    public void GetSeasonEpisodes_ReturnsAllEpisodesForSeason()
    {
        // Act
        var result = _controller.GetSeasonEpisodes(1) as OkObjectResult;
        var response = result?.Value as HateoasResponse<List<EpisodeResource>>;

        // Assert
        Assert.NotNull(response?.Data);
        Assert.Equal(6, response.Data.Count); // Season 1 has 6 episodes
        Assert.All(response.Data, episode =>
        {
            Assert.Equal(1, episode.Season);
        });
    }

    [AllureXunit]
    public void GetEpisode_ReturnsCorrectEpisodeDetails()
    {
        // Act
        var result = _controller.GetEpisode(1, 1) as OkObjectResult;
        var response = result?.Value as HateoasResponse<EpisodeResource>;

        // Assert
        Assert.NotNull(response?.Data);
        Assert.Equal(1, response.Data.Season);
        Assert.Equal(1, response.Data.EpisodeNumber);
        Assert.NotEmpty(response.Data.Title);
        Assert.NotEmpty(response.Data.ReleasedDate);
    }
}
