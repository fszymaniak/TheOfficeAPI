using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace TheOfficeAPI.Tests.E2E;

/// <summary>
/// End-to-End tests for Level 0 (RPC-style) API.
/// Tests against a deployed API instance.
/// </summary>
[Trait("Category", "E2E")]
public class Level0E2ETests : E2ETestBase
{
    private const string ApiEndpoint = "/api/v0/theOffice";
    private const string MediaType = "application/json";
    private readonly JsonSerializerOptions _jsonOptions;

    public Level0E2ETests()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task<HttpResponseMessage> SendApiRequestAsync(ApiRequest request)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, MediaType);
        return await _httpClient.PostAsync(ApiEndpoint, content);
    }

    private async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
    }

    [AllureXunit]
    public async Task GetAllSeasons_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getAllSeasons" };

        // Act
        var response = await SendApiRequestAsync(request);

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
        // Arrange
        var request = new ApiRequest { Action = "getSeasonEpisodes", Season = 1 };

        // Act
        var response = await SendApiRequestAsync(request);

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
        // Arrange
        var request = new ApiRequest { Action = "getSeasonEpisodes", Season = 999 };

        // Act
        var response = await SendApiRequestAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Season parameter is outside of the scope", apiResponse.Error);
    }

    [AllureXunit]
    public async Task GetEpisode_WithValidParameters_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getEpisode", Season = 1, Episode = 1 };

        // Act
        var response = await SendApiRequestAsync(request);

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
        // Arrange
        var request = new ApiRequest { Action = "getEpisode", Season = 999, Episode = 1 };

        // Act
        var response = await SendApiRequestAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Season parameter is outside of the scope", apiResponse.Error);
    }

    [AllureXunit]
    public async Task UnknownAction_ReturnsErrorResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "unknownAction" };

        // Act
        var response = await SendApiRequestAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Equal("Unknown action: unknownAction", apiResponse.Error);
    }
}
