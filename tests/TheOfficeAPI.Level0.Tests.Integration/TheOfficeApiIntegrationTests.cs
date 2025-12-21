using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TheOfficeAPI.Common.Models;
using TheOfficeAPI.Level0.Extensions;
using Xunit;

namespace TheOfficeAPI.Level0.Tests.Integration;

[Trait("Category", "Mocked")]
public class TheOfficeApiIntegrationTests : IClassFixture<WebApplicationFactory<TheOfficeAPI.Program>>, IDisposable
{
    private readonly WebApplicationFactory<TheOfficeAPI.Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string ApiEndpoint = "/api/v0/theOffice";
    private const string MediaType = "application/json";

    public TheOfficeApiIntegrationTests(WebApplicationFactory<TheOfficeAPI.Program> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));

        // Set environment variable before creating the client
        Environment.SetEnvironmentVariable("MATURITY_LEVEL", "Level0");

        // Configure the factory for Level0 testing
        _client = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Add Level0 services for testing
                services.AddControllers()
                    .AddApplicationPart(typeof(TheOfficeAPI.Level0.Controllers.Level0Controller).Assembly);

                services.AddLevel0Services(); // Use the new extension method
            });
        }).CreateClient();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task<HttpResponseMessage> SendApiRequestAsync(ApiRequest request)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, MediaType);
        return await _client.PostAsync(ApiEndpoint, content);
    }

    private async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();

        // Add debugging information for non-JSON responses
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

    [AllureXunit]
    public async Task GetAllSeasons_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getAllSeasons" };

        // Act
        var response = await SendApiRequestAsync(request);

        // Debug response if not OK
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"API call failed. Status: {response.StatusCode}, Content: {errorContent}");
        }

        var apiResponse = await DeserializeResponseAsync<ApiResponse<List<Season>>>(response);

        // Assert
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

    [AllureXunit]
    public async Task GetSeasonEpisodes_WithValidSeason_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getSeasonEpisodes", Season = 1 };

        // Act
        var response = await SendApiRequestAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<List<Episode>>>(response);

        // Assert
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

    [AllureXunit]
    public async Task GetSeasonEpisodes_WithNullSeason_ReturnsErrorResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getSeasonEpisodes", Season = null };

        // Act
        var response = await SendApiRequestAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        // Assert
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Equal("Season parameter is required", apiResponse.Error);
        Assert.Equal("Invalid request", apiResponse.Message);
    }

    [AllureXunit]
    public async Task GetSeasonEpisodes_WithInvalidSeason_ReturnsErrorResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getSeasonEpisodes", Season = 999 };

        // Act
        var response = await SendApiRequestAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        // Assert
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Season parameter is outside of the scope", apiResponse.Error);
        Assert.Equal("Invalid request", apiResponse.Message);
    }

    [AllureXunit]
    public async Task GetEpisode_WithValidParameters_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getEpisode", Season = 1, Episode = 1 };

        // Act
        var response = await SendApiRequestAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<Episode>>(response);

        // Assert
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(1, apiResponse.Data.Season);
        Assert.Equal(1, apiResponse.Data.EpisodeNumber);
        Assert.Equal("Episode retrieved successfully", apiResponse.Message);
    }

    [Theory]
    [InlineData(null, 1)]
    [InlineData(1, null)]
    [InlineData(null, null)]
    public async Task GetEpisode_WithNullParameters_ReturnsErrorResponse(int? season, int? episode)
    {
        // Arrange
        var request = new ApiRequest { Action = "getEpisode", Season = season, Episode = episode };

        // Act
        var response = await SendApiRequestAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        // Assert
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Equal("Both season and episode parameters are required", apiResponse.Error);
        Assert.Equal("Invalid request", apiResponse.Message);
    }

    [AllureXunit]
    public async Task GetEpisode_WithInvalidSeason_ReturnsErrorResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getEpisode", Season = 999, Episode = 1 };

        // Act
        var response = await SendApiRequestAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        // Assert
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Season parameter is outside of the scope", apiResponse.Error);
        Assert.Equal("Invalid request", apiResponse.Message);
    }

    [AllureXunit]
    public async Task GetEpisode_WithInvalidEpisode_ReturnsErrorResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "getEpisode", Season = 1, Episode = 999 };

        // Act
        var response = await SendApiRequestAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        // Assert
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Episode parameter is outside of the scope", apiResponse.Error);
        Assert.Equal("Invalid request", apiResponse.Message);
    }

    [AllureXunit]
    public async Task UnknownAction_ReturnsErrorResponse()
    {
        // Arrange
        var request = new ApiRequest { Action = "unknownAction" };

        // Act
        var response = await SendApiRequestAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await DeserializeResponseAsync<ApiResponse<object>>(response);

        // Assert
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Equal("Unknown action: unknownAction", apiResponse.Error);
        Assert.Equal("Invalid action", apiResponse.Message);
    }

    [Theory]
    [InlineData("GETALLSEASONS")]
    [InlineData("GetAllSeasons")]
    [InlineData("getallseasons")]
    public async Task Actions_AreCaseInsensitive(string action)
    {
        // Arrange
        var request = new ApiRequest { Action = action };

        // Act
        var response = await SendApiRequestAsync(request);

        // Debug the response if it's not successful
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"API call failed. Status: {response.StatusCode}, Content: {errorContent}");
        }

        var apiResponse = await DeserializeResponseAsync<ApiResponse<List<Season>>>(response);

        // Assert
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.NotEmpty(apiResponse.Data);
    }

    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }
}