# End-to-End (E2E) Tests

This project contains end-to-end integration tests for The Office API. These tests validate the API against a **deployed instance**, not an in-memory test server.

## Overview

Unlike the mocked integration tests (which use `WebApplicationFactory`), these E2E tests make **real HTTP requests** to a deployed API. They are designed to:

- Validate post-deployment functionality
- Perform smoke testing on production or staging environments
- Ensure all Richardson Maturity Model levels work correctly in deployed environments
- Catch environment-specific issues that mocked tests cannot detect

## Test Coverage

The E2E tests cover all four Richardson Maturity Model levels:

### Level 0 - RPC Style (`Level0E2ETests.cs`)
- Single endpoint (`/api/v0/theOffice`)
- Action-based requests using POST
- Tests: seasons, episodes, error handling, unknown actions

### Level 1 - Resources (`Level1E2ETests.cs`)
- Resource-based URIs (`/api/v1/seasons`, `/api/v1/seasons/{id}/episodes`)
- Still uses POST for all operations
- Tests: resource-based URIs, seasons, episodes

### Level 2 - HTTP Verbs (`Level2E2ETests.cs`)
- Proper HTTP verbs (GET, POST, PUT, DELETE)
- Correct HTTP status codes (200 OK, 404 Not Found, etc.)
- Tests: HTTP verbs, status codes, RESTful operations

### Level 3 - HATEOAS (`Level3E2ETests.cs`)
- Hypermedia links in responses
- Self-discoverable API through links
- Tests: hypermedia links, link navigation, discoverability

## Configuration

### Environment Variable Required

The E2E tests require the `API_BASE_URL` environment variable to be set:

```bash
export API_BASE_URL=https://your-api-domain.com
```

**Important:** Do NOT include a trailing slash in the URL.

### GitHub Actions Setup

To run E2E tests in GitHub Actions, configure a **repository variable** (not a secret):

1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click the **Variables** tab
4. Click **New repository variable**
5. Add the variable:
   - **Name:** `API_BASE_URL`
   - **Value:** `https://your-deployed-api.com`

The CI/CD pipeline will automatically run E2E tests if this variable is configured.

## Running Tests Locally

### Prerequisites
- .NET 9.0 SDK installed
- A deployed instance of The Office API (or local instance running on a specific port)

### Run All E2E Tests

```bash
# Set the API base URL
export API_BASE_URL=https://your-api.example.com

# Run all E2E tests
dotnet test tests/TheOfficeAPI.Tests.E2E/TheOfficeAPI.Tests.E2E.csproj
```

### Run Tests for a Specific Level

```bash
# Run only Level 0 E2E tests
dotnet test tests/TheOfficeAPI.Tests.E2E/TheOfficeAPI.Tests.E2E.csproj --filter "FullyQualifiedName~Level0E2ETests"

# Run only Level 2 E2E tests
dotnet test tests/TheOfficeAPI.Tests.E2E/TheOfficeAPI.Tests.E2E.csproj --filter "FullyQualifiedName~Level2E2ETests"

# Run only Level 3 E2E tests
dotnet test tests/TheOfficeAPI.Tests.E2E/TheOfficeAPI.Tests.E2E.csproj --filter "FullyQualifiedName~Level3E2ETests"
```

### Run Against Local Development Server

```bash
# Start your API locally (example)
cd src/TheOfficeAPI
dotnet run --urls "http://localhost:5000"

# In another terminal, run E2E tests
export API_BASE_URL=http://localhost:5000
dotnet test tests/TheOfficeAPI.Tests.E2E/TheOfficeAPI.Tests.E2E.csproj
```

## CI/CD Integration

The E2E tests are integrated into the GitHub Actions CI pipeline:

### Workflow Job: `e2e-tests`

```yaml
e2e-tests:
  runs-on: ubuntu-latest
  name: End-to-End Tests
  needs: build-and-unit-tests
  # Only run if API_BASE_URL variable is configured
  if: vars.API_BASE_URL != ''
```

**Key Points:**
- Runs after `build-and-unit-tests` job completes
- Only executes if `API_BASE_URL` repository variable is set
- Filters tests by `Category=E2E`
- Publishes test results to GitHub Actions

### When E2E Tests Run

E2E tests run automatically in the following scenarios:

1. **After successful build** (if `API_BASE_URL` is configured)
2. **On pull requests** to validate changes don't break deployed API
3. **On pushes** to main/develop branches
4. **Manual workflow dispatch** (if enabled)

### Skipping E2E Tests

If `API_BASE_URL` is not configured, the E2E tests job will be **skipped** automatically - this won't fail your CI pipeline.

## Test Architecture

### Base Class: `E2ETestBase`

All E2E test classes inherit from `E2ETestBase`, which:

- Reads `API_BASE_URL` from environment variables
- Creates an `HttpClient` configured for the deployed API
- Handles cleanup and disposal
- Throws a clear exception if `API_BASE_URL` is not set

### Test Traits

All E2E tests are marked with:

```csharp
[Trait("Category", "E2E")]
```

This allows filtering E2E tests from mocked tests:

```bash
# Run ONLY E2E tests
dotnet test --filter "Category=E2E"

# Run ONLY mocked tests
dotnet test --filter "Category=Mocked"
```

## Differences from Mocked Integration Tests

| Aspect | Mocked Integration Tests | E2E Tests |
|--------|--------------------------|-----------|
| **Target** | In-memory test server | Deployed API |
| **Framework** | `WebApplicationFactory` | `HttpClient` |
| **Environment** | Testing environment | Production/Staging |
| **Speed** | Fast (~seconds) | Slower (network latency) |
| **Dependencies** | None (self-contained) | Requires deployed API |
| **Purpose** | Validate code logic | Validate deployment |
| **Category Trait** | `[Trait("Category", "Mocked")]` | `[Trait("Category", "E2E")]` |

## Troubleshooting

### Error: "API_BASE_URL environment variable must be set"

**Solution:** Set the `API_BASE_URL` environment variable before running tests:

```bash
export API_BASE_URL=https://your-api.com
```

### Error: "Connection refused" or "Connection timeout"

**Possible causes:**
1. API is not running at the specified URL
2. Firewall blocking requests
3. Incorrect URL format

**Solutions:**
- Verify the API is accessible: `curl https://your-api.com/api/v0/theOffice`
- Check for typos in `API_BASE_URL`
- Ensure the URL uses `https://` (or `http://` for local development)

### Tests fail with 404 Not Found

**Possible causes:**
1. API endpoints have changed
2. Wrong API version configured
3. API deployment incomplete

**Solutions:**
- Verify endpoints exist manually
- Check API version compatibility
- Ensure deployment completed successfully

### Tests pass locally but fail in CI

**Possible causes:**
1. `API_BASE_URL` variable not set in GitHub Actions
2. Different API instance between local and CI
3. Network/firewall restrictions

**Solutions:**
- Verify `API_BASE_URL` is configured in GitHub repository variables
- Ensure CI can access the deployed API (check network policies)
- Review GitHub Actions logs for detailed error messages

## Best Practices

1. **Keep E2E tests focused** - Test critical user flows, not every edge case
2. **Use meaningful test names** - Clearly describe what is being tested
3. **Don't modify deployed data** - E2E tests should be read-only
4. **Handle flakiness** - Network issues can cause intermittent failures
5. **Monitor test duration** - E2E tests are slower; keep the suite lean

## Future Enhancements

Potential improvements for the E2E test suite:

- [ ] Add retry logic for transient network failures
- [ ] Implement test data setup/teardown for write operations
- [ ] Add performance benchmarking
- [ ] Create smoke test subset for faster feedback
- [ ] Add health check endpoint validation
- [ ] Implement rate limiting tests

## References

- [Richardson Maturity Model](https://martinfowler.com/articles/richardsonMaturityModel.html)
- [xUnit Documentation](https://xunit.net/)
- [ASP.NET Core Testing Best Practices](https://learn.microsoft.com/en-us/aspnet/core/test/)
