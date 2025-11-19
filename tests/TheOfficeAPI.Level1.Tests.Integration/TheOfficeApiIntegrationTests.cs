using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TheOfficeAPI.Common.Models;
using TheOfficeAPI.Level1.Extensions;
using Xunit;

namespace TheOfficeAPI.Level1.Tests.Integration;

[Trait("Category", "Mocked")]
public class TheOfficeApiIntegrationTests : IClassFixture<WebApplicationFactory<TheOfficeAPI.Program>>, IDisposable
{
    private readonly WebApplicationFactory<TheOfficeAPI.Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string SeasonsEndpoint = "/api/seasons";
    private const string EpisodesEndpoint = "/api/seasons/{0}/episodes";
    private const string EpisodeEndpoint = "/api/seasons/{0}/episodes/{1}";

    public TheOfficeApiIntegrationTests(WebApplicationFactory<TheOfficeAPI.Program> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));

        // Set environment variable before creating the client
        Environment.SetEnvironmentVariable("MATURITY_LEVEL", "Level1");

        // Configure the factory for Level1 testing
        _client = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Add Level1 services for testing
                services.AddControllers()
                    .AddApplicationPart(typeof(TheOfficeAPI.Level1.Controllers.SeasonsController).Assembly);

                services.AddLevel1Services();
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
        var response = await _client.PostAsync(SeasonsEndpoint, null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<List<Season>>>(response);

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
    public async Task GetSeasonEpisodes_WithValidSeason_ReturnsSuccessResponse()
    {
        // Act
        var endpoint = string.Format(EpisodesEndpoint, 1);
        var response = await _client.PostAsync(endpoint, null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<List<Episode>>>(response);

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
    public async Task GetSeasonEpisodes_WithInvalidSeason_ReturnsErrorResponse()
    {
        // Act
        var endpoint = string.Format(EpisodesEndpoint, 999);
        var response = await _client.PostAsync(endpoint, null);

        // Assert - Level 1 still returns 200 OK
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

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
        var response = await _client.PostAsync(endpoint, null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<Episode>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(1, apiResponse.Data.Season);
        Assert.Equal(1, apiResponse.Data.EpisodeNumber);
        Assert.Equal("Episode retrieved successfully", apiResponse.Message);
    }

    [Fact]
    public async Task GetEpisode_WithInvalidSeason_ReturnsErrorResponse()
    {
        // Act
        var endpoint = string.Format(EpisodeEndpoint, 999, 1);
        var response = await _client.PostAsync(endpoint, null);

        // Assert - Level 1 still returns 200 OK
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Season parameter is outside of the scope", apiResponse.Error);
        Assert.Equal("Invalid request", apiResponse.Message);
    }

    [Fact]
    public async Task GetEpisode_WithInvalidEpisode_ReturnsErrorResponse()
    {
        // Act
        var endpoint = string.Format(EpisodeEndpoint, 1, 999);
        var response = await _client.PostAsync(endpoint, null);

        // Assert - Level 1 still returns 200 OK
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Episode parameter is outside of the scope", apiResponse.Error);
        Assert.Equal("Invalid request", apiResponse.Message);
    }

    [Fact]
    public async Task GetAllSeasons_AlwaysReturnsHttpOk()
    {
        // Act
        var response = await _client.PostAsync(SeasonsEndpoint, null);

        // Assert - Level 1 always returns HTTP 200 OK
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ResourceBasedEndpoints_UsesDifferentUris()
    {
        // This test demonstrates the key difference of Level 1: resource-based URIs

        // Act - Get seasons
        var seasonsResponse = await _client.PostAsync(SeasonsEndpoint, null);
        var seasonsApiResponse = await DeserializeResponseAsync<ApiResponse<List<Season>>>(seasonsResponse);

        // Act - Get episodes
        var episodesEndpoint = string.Format(EpisodesEndpoint, 1);
        var episodesResponse = await _client.PostAsync(episodesEndpoint, null);
        var episodesApiResponse = await DeserializeResponseAsync<ApiResponse<List<Episode>>>(episodesResponse);

        // Assert - Different resources have different URIs (Level 1 characteristic)
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
