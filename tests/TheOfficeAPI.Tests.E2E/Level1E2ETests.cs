using System.Net;
using System.Text.Json;
using Xunit;

namespace TheOfficeAPI.Tests.E2E;

/// <summary>
/// End-to-End tests for Level 1 (Resource-based URIs) API.
/// Tests against a deployed API instance.
/// </summary>
[Trait("Category", "E2E")]
public class Level1E2ETests : E2ETestBase
{
    private const string SeasonsEndpoint = "/api/v1/seasons";
    private const string EpisodesEndpoint = "/api/v1/seasons/{0}/episodes";
    private const string EpisodeEndpoint = "/api/v1/seasons/{0}/episodes/{1}";
    private readonly JsonSerializerOptions _jsonOptions;

    public Level1E2ETests()
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

    [AllureXunit]
    public async Task GetAllSeasons_ReturnsSuccessResponse()
    {
        // Act
        var response = await _httpClient.PostAsync(SeasonsEndpoint, null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<List<Season>>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.NotEmpty(apiResponse.Data);
        Assert.Equal("Seasons retrieved successfully", apiResponse.Message);
    }

    [AllureXunit]
    public async Task GetSeasonEpisodes_WithValidSeason_ReturnsSuccessResponse()
    {
        // Act
        var endpoint = string.Format(EpisodesEndpoint, 1);
        var response = await _httpClient.PostAsync(endpoint, null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<List<Episode>>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.NotEmpty(apiResponse.Data);
        Assert.Equal("Episodes for season 1 retrieved successfully", apiResponse.Message);
    }

    [AllureXunit]
    public async Task GetSeasonEpisodes_WithInvalidSeason_ReturnsErrorResponse()
    {
        // Act
        var endpoint = string.Format(EpisodesEndpoint, 999);
        var response = await _httpClient.PostAsync(endpoint, null);

        // Assert - Level 1 still returns 200 OK for errors
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Season parameter is outside of the scope", apiResponse.Error);
    }

    [AllureXunit]
    public async Task GetEpisode_WithValidParameters_ReturnsSuccessResponse()
    {
        // Act
        var endpoint = string.Format(EpisodeEndpoint, 1, 1);
        var response = await _httpClient.PostAsync(endpoint, null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<Episode>>(response);

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(1, apiResponse.Data.Season);
        Assert.Equal(1, apiResponse.Data.EpisodeNumber);
    }

    [AllureXunit]
    public async Task GetEpisode_WithInvalidSeason_ReturnsErrorResponse()
    {
        // Act
        var endpoint = string.Format(EpisodeEndpoint, 999, 1);
        var response = await _httpClient.PostAsync(endpoint, null);

        // Assert - Level 1 returns 200 OK for errors
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Season parameter is outside of the scope", apiResponse.Error);
    }

    [AllureXunit]
    public async Task Level1_UsesResourceBasedUris()
    {
        // This test verifies Level 1 characteristic: resource-based URIs
        // Different resources have different endpoints (unlike Level 0's single endpoint)

        // Act
        var seasonsResponse = await _httpClient.PostAsync(SeasonsEndpoint, null);
        var episodesEndpoint = string.Format(EpisodesEndpoint, 1);
        var episodesResponse = await _httpClient.PostAsync(episodesEndpoint, null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, seasonsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, episodesResponse.StatusCode);
        Assert.NotEqual(SeasonsEndpoint, episodesEndpoint);
    }
}
