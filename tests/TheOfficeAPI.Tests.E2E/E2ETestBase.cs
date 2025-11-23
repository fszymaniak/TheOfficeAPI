namespace TheOfficeAPI.Tests.E2E;

/// <summary>
/// Base class for End-to-End tests against deployed API.
/// Requires API_BASE_URL environment variable to be set.
/// </summary>
public abstract class E2ETestBase : IDisposable
{
    protected readonly HttpClient _httpClient;
    protected readonly string _baseUrl;

    protected E2ETestBase()
    {
        _baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL")
            ?? throw new InvalidOperationException(
                "API_BASE_URL environment variable must be set for E2E tests. " +
                "Example: export API_BASE_URL=https://your-api.example.com");

        // Ensure URL doesn't end with slash for consistency
        _baseUrl = _baseUrl.TrimEnd('/');

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}
