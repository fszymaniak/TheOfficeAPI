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

E2E tests run automatically in the **CD (Continuous Deployment) pipeline** after successful deployment to Railway.

The `API_BASE_URL` is configured in `.github/workflows/cd.yaml` as:
```yaml
env:
  RAILWAY_URL: 'https://theofficeapi-production-5d8f.up.railway.app'
```

**No additional configuration needed** - E2E tests run automatically after each deployment to validate the deployed API.

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

The E2E tests are integrated into the **CD (Continuous Deployment) pipeline**, NOT the CI pipeline.

### Workflow Job: `e2e-tests` (in `cd.yaml`)

```yaml
e2e-tests:
  runs-on: ubuntu-latest
  name: End-to-End Tests
  needs: health-check
  env:
    API_BASE_URL: ${{ env.RAILWAY_URL }}
```

**Key Points:**
- Runs in the **CD pipeline** after successful deployment
- Executes after the `health-check` job confirms API is responding
- Tests the **newly deployed** Railway instance
- Filters tests by `Category=E2E`
- Publishes test results to GitHub Actions

### When E2E Tests Run

E2E tests run automatically after deployment in these scenarios:

1. **After Railway auto-deploys** from main/develop branches
2. **After health check passes** confirming API is accessible
3. **Before declaring deployment successful**

### Pipeline Flow

```
CI Pipeline (ci.yaml)
├── Build & Unit Tests
├── Mocked Integration Tests
└── Code Quality

CD Pipeline (cd.yaml) - Only runs on main/develop
├── Check CI Status
├── Wait for Railway Deployment
├── Health Check
├── E2E Tests ⬅️ Tests run here!
├── Live Integration Tests
├── Smoke Tests
└── CD Summary
```

**Why CD not CI?** E2E tests validate the deployed environment, so they must run AFTER deployment, not before.

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

### Tests pass locally but fail in CD

**Possible causes:**
1. Different API instance between local and Railway deployment
2. Network/firewall restrictions in GitHub Actions runners
3. Deployment not fully complete when tests run

**Solutions:**
- Verify the Railway deployment completed successfully
- Check Railway deployment logs for errors
- Ensure the `RAILWAY_URL` in `cd.yaml` is correct
- Review GitHub Actions CD logs for detailed error messages

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
