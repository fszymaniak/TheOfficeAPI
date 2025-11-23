using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TheOfficeAPI.Level3.Extensions;
using TheOfficeAPI.Level3.Models;
using Xunit;

namespace TheOfficeAPI.Level3.Tests.Integration;

[Trait("Category", "Mocked")]
public class TheOfficeApiIntegrationTests : IClassFixture<WebApplicationFactory<TheOfficeAPI.Program>>, IDisposable
{
    private readonly WebApplicationFactory<TheOfficeAPI.Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string SeasonsEndpoint = "/api/v3/seasons";
    private const string SeasonEndpoint = "/api/v3/seasons/{0}";
    private const string EpisodesEndpoint = "/api/v3/seasons/{0}/episodes";
    private const string EpisodeEndpoint = "/api/v3/seasons/{0}/episodes/{1}";

    public TheOfficeApiIntegrationTests(WebApplicationFactory<TheOfficeAPI.Program> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));

        // Set environment variable before creating the client
        Environment.SetEnvironmentVariable("MATURITY_LEVEL", "Level3");

        // Configure the factory for Level3 testing
        _client = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Add Level3 services for testing
                services.AddControllers()
                    .AddApplicationPart(typeof(TheOfficeAPI.Level3.Controllers.SeasonsController).Assembly);

                services.AddLevel3Services();
            });
        }).CreateClient();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!IsValidJson(responseContent))
        {
            throw new InvalidOperationException(
                $"API returned non-JSON response. Status: {response.StatusCode}, Content: {responseContent}");
        }

        return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
    }

    private static bool IsValidJson(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return false;

        content = content.Trim();
        return (content.StartsWith("{") && content.EndsWith("}")) ||
               (content.StartsWith("[") && content.EndsWith("]"));
    }

    [Fact]
    public async Task GetAllSeasons_ReturnsSuccessResponse()
    {
        // Act
        var response = await _client.GetAsync(SeasonsEndpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<HateoasResponse<List<SeasonResource>>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.NotEmpty(apiResponse.Data);
        Assert.Equal("Seasons retrieved successfully", apiResponse.Message);
        Assert.All(apiResponse.Data, season =>
        {
            Assert.NotEmpty(season.SeasonNumber);
            Assert.True(season.EpisodeCount > 0);
        });
    }

    [Fact]
    public async Task GetAllSeasons_IncludesHypermediaLinks()
    {
        // Act
        var response = await _client.GetAsync(SeasonsEndpoint);

        // Assert
        var apiResponse = await DeserializeResponseAsync<HateoasResponse<List<SeasonResource>>>(response);

        Assert.NotNull(apiResponse);
        Assert.NotNull(apiResponse.Links);
        Assert.NotEmpty(apiResponse.Links);

        // Check response-level links
        var selfLink = apiResponse.Links.FirstOrDefault(l => l.Rel == "self");
        Assert.NotNull(selfLink);
        Assert.Equal("/api/v3/seasons", selfLink.Href);
        Assert.Equal("GET", selfLink.Method);

        // Check each season resource has links
        Assert.All(apiResponse.Data!, season =>
        {
            Assert.NotNull(season.Links);
            Assert.NotEmpty(season.Links);

            var seasonSelfLink = season.Links.FirstOrDefault(l => l.Rel == "self");
            Assert.NotNull(seasonSelfLink);
            Assert.Contains($"/api/v3/seasons/{season.SeasonNumber}", seasonSelfLink.Href);

            var episodesLink = season.Links.FirstOrDefault(l => l.Rel == "episodes");
            Assert.NotNull(episodesLink);
            Assert.Contains($"/api/v3/seasons/{season.SeasonNumber}/episodes", episodesLink.Href);
        });
    }

    [Fact]
    public async Task GetSeason_WithValidSeasonNumber_ReturnsSuccessResponse()
    {
        // Act
        var endpoint = string.Format(SeasonEndpoint, 1);
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<HateoasResponse<SeasonResource>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal("1", apiResponse.Data.SeasonNumber);
        Assert.Equal("Season 1 retrieved successfully", apiResponse.Message);
    }

    [Fact]
    public async Task GetSeason_IncludesNavigationLinks()
    {
        // Act
        var endpoint = string.Format(SeasonEndpoint, 2);
        var response = await _client.GetAsync(endpoint);

        // Assert
        var apiResponse = await DeserializeResponseAsync<HateoasResponse<SeasonResource>>(response);

        Assert.NotNull(apiResponse);
        Assert.NotNull(apiResponse.Data);
        Assert.NotNull(apiResponse.Data.Links);
        Assert.NotEmpty(apiResponse.Data.Links);

        // Check for self link
        var selfLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "self");
        Assert.NotNull(selfLink);
        Assert.Equal("/api/v3/seasons/2", selfLink.Href);

        // Check for episodes link
        var episodesLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "episodes");
        Assert.NotNull(episodesLink);
        Assert.Equal("/api/v3/seasons/2/episodes", episodesLink.Href);

        // Check for collection link
        var collectionLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "collection");
        Assert.NotNull(collectionLink);
        Assert.Equal("/api/v3/seasons", collectionLink.Href);
    }

    [Fact]
    public async Task GetSeason_WithInvalidSeasonNumber_Returns404NotFound()
    {
        // Act
        var endpoint = string.Format(SeasonEndpoint, 999);
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<HateoasResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Season parameter is outside of the scope", apiResponse.Error);
        Assert.Equal("Invalid request", apiResponse.Message);

        // Even error responses should include helpful links
        Assert.NotNull(apiResponse.Links);
        var collectionLink = apiResponse.Links.FirstOrDefault(l => l.Rel == "collection");
        Assert.NotNull(collectionLink);
    }

    [Fact]
    public async Task GetSeasonEpisodes_WithValidSeason_ReturnsSuccessResponse()
    {
        // Act
        var endpoint = string.Format(EpisodesEndpoint, 1);
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<HateoasResponse<List<EpisodeResource>>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.NotEmpty(apiResponse.Data);
        Assert.Equal("Episodes for season 1 retrieved successfully", apiResponse.Message);
        Assert.All(apiResponse.Data, episode =>
        {
            Assert.Equal(1, episode.Season);
            Assert.NotNull(episode.EpisodeNumber);
            Assert.True(episode.EpisodeNumber > 0);
        });
    }

    [Fact]
    public async Task GetSeasonEpisodes_IncludesHypermediaLinks()
    {
        // Act
        var endpoint = string.Format(EpisodesEndpoint, 1);
        var response = await _client.GetAsync(endpoint);

        // Assert
        var apiResponse = await DeserializeResponseAsync<HateoasResponse<List<EpisodeResource>>>(response);

        Assert.NotNull(apiResponse);
        Assert.NotNull(apiResponse.Links);
        Assert.NotEmpty(apiResponse.Links);

        // Check response-level links
        var selfLink = apiResponse.Links.FirstOrDefault(l => l.Rel == "self");
        Assert.NotNull(selfLink);

        var seasonLink = apiResponse.Links.FirstOrDefault(l => l.Rel == "season");
        Assert.NotNull(seasonLink);

        var collectionLink = apiResponse.Links.FirstOrDefault(l => l.Rel == "collection");
        Assert.NotNull(collectionLink);

        // Check each episode has links
        Assert.All(apiResponse.Data!, episode =>
        {
            Assert.NotNull(episode.Links);
            Assert.NotEmpty(episode.Links);

            var episodeSelfLink = episode.Links.FirstOrDefault(l => l.Rel == "self");
            Assert.NotNull(episodeSelfLink);
        });
    }

    [Fact]
    public async Task GetSeasonEpisodes_WithInvalidSeason_Returns404NotFound()
    {
        // Act
        var endpoint = string.Format(EpisodesEndpoint, 999);
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<HateoasResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Season parameter is outside of the scope", apiResponse.Error);
        Assert.Equal("Invalid request", apiResponse.Message);
    }

    [Fact]
    public async Task GetEpisode_WithValidParameters_ReturnsSuccessResponse()
    {
        // Act
        var endpoint = string.Format(EpisodeEndpoint, 1, 1);
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<HateoasResponse<EpisodeResource>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(1, apiResponse.Data.Season);
        Assert.Equal(1, apiResponse.Data.EpisodeNumber);
        Assert.Equal("Episode retrieved successfully", apiResponse.Message);
    }

    [Fact]
    public async Task GetEpisode_IncludesNextAndPreviousLinks()
    {
        // Act - Get middle episode
        var endpoint = string.Format(EpisodeEndpoint, 2, 5);
        var response = await _client.GetAsync(endpoint);

        // Assert
        var apiResponse = await DeserializeResponseAsync<HateoasResponse<EpisodeResource>>(response);

        Assert.NotNull(apiResponse);
        Assert.NotNull(apiResponse.Data);
        Assert.NotNull(apiResponse.Data.Links);

        // Check for next link
        var nextLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "next");
        Assert.NotNull(nextLink);
        Assert.Equal("/api/v3/seasons/2/episodes/6", nextLink.Href);

        // Check for previous link
        var previousLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "previous");
        Assert.NotNull(previousLink);
        Assert.Equal("/api/v3/seasons/2/episodes/4", previousLink.Href);

        // Check for parent links
        var seasonLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "season");
        Assert.NotNull(seasonLink);

        var episodesLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "episodes");
        Assert.NotNull(episodesLink);

        var collectionLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "collection");
        Assert.NotNull(collectionLink);
    }

    [Fact]
    public async Task GetFirstEpisode_HasNoPreviousLink()
    {
        // Act - Get first episode
        var endpoint = string.Format(EpisodeEndpoint, 1, 1);
        var response = await _client.GetAsync(endpoint);

        // Assert
        var apiResponse = await DeserializeResponseAsync<HateoasResponse<EpisodeResource>>(response);

        Assert.NotNull(apiResponse);
        Assert.NotNull(apiResponse.Data);
        Assert.NotNull(apiResponse.Data.Links);

        // Check that previous link does not exist
        var previousLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "previous");
        Assert.Null(previousLink);

        // But next link should exist
        var nextLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "next");
        Assert.NotNull(nextLink);
    }

    [Fact]
    public async Task GetEpisode_WithInvalidSeason_Returns404NotFound()
    {
        // Act
        var endpoint = string.Format(EpisodeEndpoint, 999, 1);
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<HateoasResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Season parameter is outside of the scope", apiResponse.Error);
        Assert.Equal("Invalid request", apiResponse.Message);
    }

    [Fact]
    public async Task GetEpisode_WithInvalidEpisode_Returns404NotFound()
    {
        // Act
        var endpoint = string.Format(EpisodeEndpoint, 1, 999);
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<HateoasResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Episode parameter is outside of the scope", apiResponse.Error);
        Assert.Equal("Invalid request", apiResponse.Message);

        // Error response should include helpful links
        Assert.NotNull(apiResponse.Links);
        Assert.NotEmpty(apiResponse.Links);
    }

    [Fact]
    public async Task Level3_UsesProperHttpVerbs()
    {
        // This test demonstrates that Level 3 maintains proper HTTP verbs from Level 2

        // Act - All requests use GET for retrieval
        var seasonsResponse = await _client.GetAsync(SeasonsEndpoint);
        var episodesEndpoint = string.Format(EpisodesEndpoint, 1);
        var episodesResponse = await _client.GetAsync(episodesEndpoint);

        // Assert - Level 3 uses GET like Level 2
        Assert.Equal(HttpStatusCode.OK, seasonsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, episodesResponse.StatusCode);
    }

    [Fact]
    public async Task Level3_UsesProperHttpStatusCodes()
    {
        // This test demonstrates Level 3 maintains proper HTTP status codes from Level 2

        // Act - Valid request
        var validResponse = await _client.GetAsync(SeasonsEndpoint);

        // Act - Invalid request
        var invalidEndpoint = string.Format(SeasonEndpoint, 999);
        var invalidResponse = await _client.GetAsync(invalidEndpoint);

        // Assert - Level 3 uses proper status codes (200 OK, 404 Not Found)
        Assert.Equal(HttpStatusCode.OK, validResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, invalidResponse.StatusCode);
    }

    [Fact]
    public async Task Level3_HATEOAS_LinksEnableDiscoverability()
    {
        // This test demonstrates the key feature of Level 3: hypermedia links enable API discoverability

        // Act - Start from the root seasons endpoint
        var seasonsResponse = await _client.GetAsync(SeasonsEndpoint);
        var seasonsApiResponse = await DeserializeResponseAsync<HateoasResponse<List<SeasonResource>>>(seasonsResponse);

        // Assert - We can discover episodes link from seasons
        Assert.NotNull(seasonsApiResponse?.Data);
        var firstSeason = seasonsApiResponse.Data.First();
        var episodesLink = firstSeason.Links.FirstOrDefault(l => l.Rel == "episodes");
        Assert.NotNull(episodesLink);

        // Act - Follow the episodes link
        var episodesResponse = await _client.GetAsync(episodesLink.Href);
        var episodesApiResponse = await DeserializeResponseAsync<HateoasResponse<List<EpisodeResource>>>(episodesResponse);

        // Assert - We can discover individual episode links from episodes collection
        Assert.NotNull(episodesApiResponse?.Data);
        var firstEpisode = episodesApiResponse.Data.First();
        var episodeLink = firstEpisode.Links.FirstOrDefault(l => l.Rel == "self");
        Assert.NotNull(episodeLink);

        // Act - Follow the episode link
        var episodeResponse = await _client.GetAsync(episodeLink.Href);

        // Assert - We successfully navigated through the API using only hypermedia links
        Assert.Equal(HttpStatusCode.OK, episodeResponse.StatusCode);
    }

    [Fact]
    public async Task ResourceBasedEndpoints_UsesDifferentUris()
    {
        // This test demonstrates that Level 3 maintains resource-based URIs from Levels 1 and 2

        // Act - Get seasons
        var seasonsResponse = await _client.GetAsync(SeasonsEndpoint);
        var seasonsApiResponse = await DeserializeResponseAsync<HateoasResponse<List<SeasonResource>>>(seasonsResponse);

        // Act - Get episodes
        var episodesEndpoint = string.Format(EpisodesEndpoint, 1);
        var episodesResponse = await _client.GetAsync(episodesEndpoint);
        var episodesApiResponse = await DeserializeResponseAsync<HateoasResponse<List<EpisodeResource>>>(episodesResponse);

        // Assert - Different resources have different URIs
        Assert.True(seasonsApiResponse?.Success);
        Assert.True(episodesApiResponse?.Success);
        Assert.NotEqual(SeasonsEndpoint, episodesEndpoint);
    }

    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }
}
