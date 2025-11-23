using System.Net;
using System.Text.Json;
using Xunit;

namespace TheOfficeAPI.Tests.E2E;

/// <summary>
/// End-to-End tests for Level 2 (HTTP Verbs and Status Codes) API.
/// Tests against a deployed API instance.
/// </summary>
[Trait("Category", "E2E")]
public class Level2E2ETests : E2ETestBase
{
    private const string SeasonsEndpoint = "/api/v2/seasons";
    private const string SeasonEndpoint = "/api/v2/seasons/{0}";
    private const string EpisodesEndpoint = "/api/v2/seasons/{0}/episodes";
    private const string EpisodeEndpoint = "/api/v2/seasons/{0}/episodes/{1}";
    private readonly JsonSerializerOptions _jsonOptions;

    public Level2E2ETests()
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

        var apiResponse = await DeserializeResponseAsync<ApiResponse<List<Season>>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.NotEmpty(apiResponse.Data);
        Assert.Equal("Seasons retrieved successfully", apiResponse.Message);
    }

    [Fact]
    public async Task GetSeason_WithValidSeasonNumber_ReturnsSuccessResponse()
    {
        // Act
        var endpoint = string.Format(SeasonEndpoint, 1);
        var response = await _httpClient.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<Season>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal("1", apiResponse.Data.SeasonNumber);
    }

    [Fact]
    public async Task GetSeason_WithInvalidSeasonNumber_Returns404NotFound()
    {
        // Act
        var endpoint = string.Format(SeasonEndpoint, 999);
        var response = await _httpClient.GetAsync(endpoint);

        // Assert - Level 2 returns proper HTTP status codes
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

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

        var apiResponse = await DeserializeResponseAsync<ApiResponse<List<Episode>>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.NotEmpty(apiResponse.Data);
        Assert.Equal("Episodes for season 1 retrieved successfully", apiResponse.Message);
    }

    [Fact]
    public async Task GetSeasonEpisodes_WithInvalidSeason_Returns404NotFound()
    {
        // Act
        var endpoint = string.Format(EpisodesEndpoint, 999);
        var response = await _httpClient.GetAsync(endpoint);

        // Assert - Level 2 returns 404 Not Found
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Season parameter is outside of the scope", apiResponse.Error);
    }

    [Fact]
    public async Task GetEpisode_WithValidParameters_ReturnsSuccessResponse()
    {
        // Act
        var endpoint = string.Format(EpisodeEndpoint, 1, 1);
        var response = await _httpClient.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<Episode>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(1, apiResponse.Data.Season);
        Assert.Equal(1, apiResponse.Data.EpisodeNumber);
    }

    [Fact]
    public async Task GetEpisode_WithInvalidSeason_Returns404NotFound()
    {
        // Act
        var endpoint = string.Format(EpisodeEndpoint, 999, 1);
        var response = await _httpClient.GetAsync(endpoint);

        // Assert - Level 2 returns 404 Not Found
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Season parameter is outside of the scope", apiResponse.Error);
    }

    [Fact]
    public async Task GetEpisode_WithInvalidEpisode_Returns404NotFound()
    {
        // Act
        var endpoint = string.Format(EpisodeEndpoint, 1, 999);
        var response = await _httpClient.GetAsync(endpoint);

        // Assert - Level 2 returns 404 Not Found
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Episode parameter is outside of the scope", apiResponse.Error);
    }

    [Fact]
    public async Task Level2_UsesProperHttpVerbs()
    {
        // This test demonstrates Level 2 characteristic: proper HTTP verbs
        // All read operations use GET instead of POST

        // Act
        var seasonsResponse = await _httpClient.GetAsync(SeasonsEndpoint);
        var episodesEndpoint = string.Format(EpisodesEndpoint, 1);
        var episodesResponse = await _httpClient.GetAsync(episodesEndpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, seasonsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, episodesResponse.StatusCode);
    }

    [Fact]
    public async Task Level2_UsesProperHttpStatusCodes()
    {
        // This test demonstrates Level 2's use of proper HTTP status codes
        // Valid requests return 200 OK, invalid requests return 404 Not Found

        // Act - Valid request
        var validResponse = await _httpClient.GetAsync(SeasonsEndpoint);

        // Act - Invalid request
        var invalidEndpoint = string.Format(SeasonEndpoint, 999);
        var invalidResponse = await _httpClient.GetAsync(invalidEndpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, validResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, invalidResponse.StatusCode);
    }
}
