using System.Net;
using System.Text.Json;

namespace TheOfficeAPI.Tests.E2E;

/// <summary>
/// End-to-End tests for Level 3 (HATEOAS) API.
/// Tests against a deployed API instance.
/// </summary>
[Trait("Category", "E2E")]
public class Level3E2ETests : E2ETestBase
{
    private const string SeasonsEndpoint = "/api/v3/seasons";
    private const string SeasonEndpoint = "/api/v3/seasons/{0}";
    private const string EpisodesEndpoint = "/api/v3/seasons/{0}/episodes";
    private const string EpisodeEndpoint = "/api/v3/seasons/{0}/episodes/{1}";
    private readonly JsonSerializerOptions _jsonOptions;

    public Level3E2ETests()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
    }

    [Fact]
    public async Task GetAllSeasons_ReturnsSuccessResponse()
    {
        // Act
        var response = await _httpClient.GetAsync(SeasonsEndpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<HateoasResponse<List<SeasonResource>>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.NotEmpty(apiResponse.Data);
        Assert.Equal("Seasons retrieved successfully", apiResponse.Message);
    }

    [Fact]
    public async Task GetAllSeasons_IncludesHypermediaLinks()
    {
        // Act
        var response = await _httpClient.GetAsync(SeasonsEndpoint);

        // Assert
        var apiResponse = await DeserializeResponseAsync<HateoasResponse<List<SeasonResource>>>(response);

        Assert.NotNull(apiResponse);
        Assert.NotNull(apiResponse.Links);
        Assert.NotEmpty(apiResponse.Links);

        // Verify self link exists
        var selfLink = apiResponse.Links.FirstOrDefault(l => l.Rel == "self");
        Assert.NotNull(selfLink);
        Assert.Contains("/api/v3/seasons", selfLink.Href);
    }

    [Fact]
    public async Task GetAllSeasons_EachSeasonIncludesHypermediaLinks()
    {
        // Act
        var response = await _httpClient.GetAsync(SeasonsEndpoint);

        // Assert
        var apiResponse = await DeserializeResponseAsync<HateoasResponse<List<SeasonResource>>>(response);

        Assert.NotNull(apiResponse);
        Assert.NotNull(apiResponse.Data);
        Assert.All(apiResponse.Data, season =>
        {
            Assert.NotNull(season.Links);
            Assert.NotEmpty(season.Links);

            // Each season should have self and episodes links
            Assert.Contains(season.Links, l => l.Rel == "self");
            Assert.Contains(season.Links, l => l.Rel == "episodes");
        });
    }

    [Fact]
    public async Task GetSeason_WithValidSeasonNumber_ReturnsSuccessResponse()
    {
        // Act
        var endpoint = string.Format(SeasonEndpoint, 1);
        var response = await _httpClient.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<HateoasResponse<SeasonResource>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal("1", apiResponse.Data.SeasonNumber);
    }

    [Fact]
    public async Task GetSeason_IncludesHypermediaLinks()
    {
        // Act
        var endpoint = string.Format(SeasonEndpoint, 1);
        var response = await _httpClient.GetAsync(endpoint);

        // Assert
        var apiResponse = await DeserializeResponseAsync<HateoasResponse<SeasonResource>>(response);

        Assert.NotNull(apiResponse);
        Assert.NotNull(apiResponse.Data);
        Assert.NotNull(apiResponse.Data.Links);
        Assert.NotEmpty(apiResponse.Data.Links);

        // Verify self and episodes links exist
        var selfLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "self");
        var episodesLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "episodes");

        Assert.NotNull(selfLink);
        Assert.NotNull(episodesLink);
        Assert.Contains("/api/v3/seasons/1", selfLink.Href);
        Assert.Contains("/api/v3/seasons/1/episodes", episodesLink.Href);
    }

    [Fact]
    public async Task GetSeason_WithInvalidSeasonNumber_Returns404NotFound()
    {
        // Act
        var endpoint = string.Format(SeasonEndpoint, 999);
        var response = await _httpClient.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<HateoasResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Season parameter is outside of the scope", apiResponse.Error);
    }

    [Fact]
    public async Task GetSeasonEpisodes_WithValidSeason_ReturnsSuccessResponse()
    {
        // Act
        var endpoint = string.Format(EpisodesEndpoint, 1);
        var response = await _httpClient.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<HateoasResponse<List<EpisodeResource>>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.NotEmpty(apiResponse.Data);
        Assert.Equal("Episodes for season 1 retrieved successfully", apiResponse.Message);
    }

    [Fact]
    public async Task GetSeasonEpisodes_EachEpisodeIncludesHypermediaLinks()
    {
        // Act
        var endpoint = string.Format(EpisodesEndpoint, 1);
        var response = await _httpClient.GetAsync(endpoint);

        // Assert
        var apiResponse = await DeserializeResponseAsync<HateoasResponse<List<EpisodeResource>>>(response);

        Assert.NotNull(apiResponse);
        Assert.NotNull(apiResponse.Data);
        Assert.All(apiResponse.Data, episode =>
        {
            Assert.NotNull(episode.Links);
            Assert.NotEmpty(episode.Links);

            // Each episode should have self and season links
            Assert.Contains(episode.Links, l => l.Rel == "self");
            Assert.Contains(episode.Links, l => l.Rel == "season");
        });
    }

    [Fact]
    public async Task GetEpisode_WithValidParameters_ReturnsSuccessResponse()
    {
        // Act
        var endpoint = string.Format(EpisodeEndpoint, 1, 1);
        var response = await _httpClient.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<HateoasResponse<EpisodeResource>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(1, apiResponse.Data.Season);
        Assert.Equal(1, apiResponse.Data.EpisodeNumber);
    }

    [Fact]
    public async Task GetEpisode_IncludesHypermediaLinks()
    {
        // Act
        var endpoint = string.Format(EpisodeEndpoint, 1, 1);
        var response = await _httpClient.GetAsync(endpoint);

        // Assert
        var apiResponse = await DeserializeResponseAsync<HateoasResponse<EpisodeResource>>(response);

        Assert.NotNull(apiResponse);
        Assert.NotNull(apiResponse.Data);
        Assert.NotNull(apiResponse.Data.Links);
        Assert.NotEmpty(apiResponse.Data.Links);

        // Verify self and season links exist
        var selfLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "self");
        var seasonLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "season");

        Assert.NotNull(selfLink);
        Assert.NotNull(seasonLink);
        Assert.Contains("/api/v3/seasons/1/episodes/1", selfLink.Href);
        Assert.Contains("/api/v3/seasons/1", seasonLink.Href);
    }

    [Fact]
    public async Task GetEpisode_WithInvalidSeason_Returns404NotFound()
    {
        // Act
        var endpoint = string.Format(EpisodeEndpoint, 999, 1);
        var response = await _httpClient.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<HateoasResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Season parameter is outside of the scope", apiResponse.Error);
    }

    [Fact]
    public async Task Level3_LinksAreNavigable()
    {
        // This test demonstrates HATEOAS: following hypermedia links to navigate the API

        // Act 1: Get all seasons
        var seasonsResponse = await _httpClient.GetAsync(SeasonsEndpoint);
        var seasonsApiResponse = await DeserializeResponseAsync<HateoasResponse<List<SeasonResource>>>(seasonsResponse);

        Assert.NotNull(seasonsApiResponse);
        Assert.NotNull(seasonsApiResponse.Data);
        Assert.NotEmpty(seasonsApiResponse.Data);

        // Act 2: Follow the "episodes" link from the first season
        var firstSeason = seasonsApiResponse.Data.First();
        var episodesLink = firstSeason.Links.FirstOrDefault(l => l.Rel == "episodes");
        Assert.NotNull(episodesLink);

        // The link should be absolute, but if it's relative, we need to construct the full URL
        var episodesUrl = episodesLink.Href.StartsWith("http")
            ? episodesLink.Href
            : episodesLink.Href;

        var episodesResponse = await _httpClient.GetAsync(episodesUrl);
        var episodesApiResponse = await DeserializeResponseAsync<HateoasResponse<List<EpisodeResource>>>(episodesResponse);

        // Assert: We successfully navigated using hypermedia links
        Assert.Equal(HttpStatusCode.OK, episodesResponse.StatusCode);
        Assert.NotNull(episodesApiResponse);
        Assert.True(episodesApiResponse.Success);
        Assert.NotNull(episodesApiResponse.Data);
        Assert.NotEmpty(episodesApiResponse.Data);
    }
}
