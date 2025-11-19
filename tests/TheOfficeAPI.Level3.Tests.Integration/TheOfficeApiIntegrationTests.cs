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
    private const string SeasonsEndpoint = "/api/seasons";
    private const string SeasonEndpoint = "/api/seasons/{0}";
    private const string EpisodesEndpoint = "/api/seasons/{0}/episodes";
    private const string EpisodeEndpoint = "/api/seasons/{0}/episodes/{1}";

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
    public async Task GetAllSeasons_ReturnsSuccessResponseWithHATEOAS()
    {
        // Act
        var response = await _client.GetAsync(SeasonsEndpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponseWithLinks<List<SeasonWithLinks>>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.NotEmpty(apiResponse.Data);
        Assert.Equal("Seasons retrieved successfully", apiResponse.Message);

        // Assert - Response includes HATEOAS links
        Assert.NotNull(apiResponse.Links);
        Assert.NotEmpty(apiResponse.Links);
        Assert.Contains(apiResponse.Links, link => link.Rel == "self");
    }

    [Fact]
    public async Task GetAllSeasons_EachSeasonIncludesHATEOASLinks()
    {
        // Act
        var response = await _client.GetAsync(SeasonsEndpoint);
        var apiResponse = await DeserializeResponseAsync<ApiResponseWithLinks<List<SeasonWithLinks>>>(response);

        // Assert - Each season includes links
        Assert.NotNull(apiResponse?.Data);
        foreach (var season in apiResponse.Data)
        {
            Assert.NotNull(season.Links);
            Assert.NotEmpty(season.Links);
            Assert.Contains(season.Links, link => link.Rel == "self");
            Assert.Contains(season.Links, link => link.Rel == "episodes");
            Assert.Contains(season.Links, link => link.Rel == "allSeasons");
        }
    }

    [Fact]
    public async Task GetSeason_WithValidSeasonNumber_ReturnsSuccessResponseWithHATEOAS()
    {
        // Act
        var endpoint = string.Format(SeasonEndpoint, 2);
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponseWithLinks<SeasonWithLinks>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal("2", apiResponse.Data.SeasonNumber);

        // Assert - Includes HATEOAS links
        Assert.NotNull(apiResponse.Data.Links);
        Assert.NotEmpty(apiResponse.Data.Links);
        Assert.Contains(apiResponse.Data.Links, link => link.Rel == "self");
        Assert.Contains(apiResponse.Data.Links, link => link.Rel == "episodes");
        Assert.Contains(apiResponse.Data.Links, link => link.Rel == "previous");
        Assert.Contains(apiResponse.Data.Links, link => link.Rel == "next");
    }

    [Fact]
    public async Task GetSeason_WithInvalidSeasonNumber_Returns404WithHelpfulLinks()
    {
        // Act
        var endpoint = string.Format(SeasonEndpoint, 999);
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponseWithLinks<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);

        // Assert - Even error responses include helpful HATEOAS links
        Assert.NotNull(apiResponse.Links);
        Assert.NotEmpty(apiResponse.Links);
        Assert.Contains(apiResponse.Links, link => link.Rel == "allSeasons");
    }

    [Fact]
    public async Task GetSeasonEpisodes_WithValidSeason_ReturnsSuccessResponseWithHATEOAS()
    {
        // Act
        var endpoint = string.Format(EpisodesEndpoint, 1);
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponseWithLinks<List<EpisodeWithLinks>>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.NotEmpty(apiResponse.Data);

        // Assert - Response includes HATEOAS links
        Assert.NotNull(apiResponse.Links);
        Assert.NotEmpty(apiResponse.Links);
        Assert.Contains(apiResponse.Links, link => link.Rel == "self");
        Assert.Contains(apiResponse.Links, link => link.Rel == "season");
        Assert.Contains(apiResponse.Links, link => link.Rel == "allSeasons");
    }

    [Fact]
    public async Task GetSeasonEpisodes_EachEpisodeIncludesHATEOASLinks()
    {
        // Act
        var endpoint = string.Format(EpisodesEndpoint, 1);
        var response = await _client.GetAsync(endpoint);
        var apiResponse = await DeserializeResponseAsync<ApiResponseWithLinks<List<EpisodeWithLinks>>>(response);

        // Assert - Each episode includes links
        Assert.NotNull(apiResponse?.Data);
        foreach (var episode in apiResponse.Data)
        {
            Assert.NotNull(episode.Links);
            Assert.NotEmpty(episode.Links);
            Assert.Contains(episode.Links, link => link.Rel == "self");
            Assert.Contains(episode.Links, link => link.Rel == "season");
            Assert.Contains(episode.Links, link => link.Rel == "allEpisodes");
        }
    }

    [Fact]
    public async Task GetEpisode_WithValidParameters_ReturnsSuccessResponseWithHATEOAS()
    {
        // Act
        var endpoint = string.Format(EpisodeEndpoint, 1, 1);
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponseWithLinks<EpisodeWithLinks>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);

        // Assert - Episode includes HATEOAS links
        Assert.NotNull(apiResponse.Data.Links);
        Assert.NotEmpty(apiResponse.Data.Links);
        Assert.Contains(apiResponse.Data.Links, link => link.Rel == "self");
        Assert.Contains(apiResponse.Data.Links, link => link.Rel == "season");
        Assert.Contains(apiResponse.Data.Links, link => link.Rel == "allEpisodes");
    }

    [Fact]
    public async Task GetEpisode_WithInvalidParameters_Returns404WithHelpfulLinks()
    {
        // Act
        var endpoint = string.Format(EpisodeEndpoint, 1, 999);
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponseWithLinks<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);

        // Assert - Error response includes helpful HATEOAS links
        Assert.NotNull(apiResponse.Links);
        Assert.NotEmpty(apiResponse.Links);
        Assert.Contains(apiResponse.Links, link => link.Rel == "allEpisodes");
        Assert.Contains(apiResponse.Links, link => link.Rel == "season");
        Assert.Contains(apiResponse.Links, link => link.Rel == "allSeasons");
    }

    [Fact]
    public async Task Level3_UsesProperHttpVerbsAndStatusCodes()
    {
        // This test demonstrates Level 3 maintains Level 2 features

        // Act - Valid request
        var validResponse = await _client.GetAsync(SeasonsEndpoint);

        // Act - Invalid request
        var invalidEndpoint = string.Format(SeasonEndpoint, 999);
        var invalidResponse = await _client.GetAsync(invalidEndpoint);

        // Assert - Uses GET and proper status codes
        Assert.Equal(HttpStatusCode.OK, validResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, invalidResponse.StatusCode);
    }

    [Fact]
    public async Task Level3_LinksEnableAPIDiscovery()
    {
        // This test demonstrates the key HATEOAS feature: API discovery through links

        // Act - Start from root
        var seasonsResponse = await _client.GetAsync(SeasonsEndpoint);
        var seasonsApiResponse = await DeserializeResponseAsync<ApiResponseWithLinks<List<SeasonWithLinks>>>(seasonsResponse);

        // Follow link to first season
        Assert.NotNull(seasonsApiResponse?.Data);
        var firstSeason = seasonsApiResponse.Data.First();
        var seasonLink = firstSeason.Links.FirstOrDefault(l => l.Rel == "self");
        Assert.NotNull(seasonLink);

        var seasonResponse = await _client.GetAsync(seasonLink.Href);
        var seasonApiResponse = await DeserializeResponseAsync<ApiResponseWithLinks<SeasonWithLinks>>(seasonResponse);

        // Follow link to episodes
        Assert.NotNull(seasonApiResponse?.Data);
        var episodesLink = seasonApiResponse.Data.Links.FirstOrDefault(l => l.Rel == "episodes");
        Assert.NotNull(episodesLink);

        var episodesResponse = await _client.GetAsync(episodesLink.Href);
        var episodesApiResponse = await DeserializeResponseAsync<ApiResponseWithLinks<List<EpisodeWithLinks>>>(episodesResponse);

        // Follow link to specific episode
        Assert.NotNull(episodesApiResponse?.Data);
        var firstEpisode = episodesApiResponse.Data.First();
        var episodeLink = firstEpisode.Links.FirstOrDefault(l => l.Rel == "self");
        Assert.NotNull(episodeLink);

        var episodeResponse = await _client.GetAsync(episodeLink.Href);

        // Assert - Successfully navigated the API using only links
        Assert.Equal(HttpStatusCode.OK, episodeResponse.StatusCode);
    }

    [Fact]
    public async Task Level3_NavigationLinksWorkCorrectly()
    {
        // Act - Get season 2
        var endpoint = string.Format(SeasonEndpoint, 2);
        var response = await _client.GetAsync(endpoint);
        var apiResponse = await DeserializeResponseAsync<ApiResponseWithLinks<SeasonWithLinks>>(response);

        // Follow previous link
        Assert.NotNull(apiResponse?.Data);
        var previousLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "previous");
        Assert.NotNull(previousLink);

        var previousResponse = await _client.GetAsync(previousLink.Href);
        var previousApiResponse = await DeserializeResponseAsync<ApiResponseWithLinks<SeasonWithLinks>>(previousResponse);

        // Assert - Previous link leads to season 1
        Assert.NotNull(previousApiResponse?.Data);
        Assert.Equal("1", previousApiResponse.Data.SeasonNumber);

        // Follow next link from original season 2
        var nextLink = apiResponse.Data.Links.FirstOrDefault(l => l.Rel == "next");
        Assert.NotNull(nextLink);

        var nextResponse = await _client.GetAsync(nextLink.Href);
        var nextApiResponse = await DeserializeResponseAsync<ApiResponseWithLinks<SeasonWithLinks>>(nextResponse);

        // Assert - Next link leads to season 3
        Assert.NotNull(nextApiResponse?.Data);
        Assert.Equal("3", nextApiResponse.Data.SeasonNumber);
    }

    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }
}
