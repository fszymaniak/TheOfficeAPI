# The Office API

A REST API providing information about episodes and seasons from the TV show "The Office", designed to demonstrate the **Richardson Maturity Model** for RESTful API design.

## Overview

The Office API is an educational project that implements all four levels of the Richardson Maturity Model, showing the evolution from basic RPC-style APIs (Level 0) to fully RESTful, HATEOAS-compliant APIs (Level 3). Each implementation level is available simultaneously, allowing direct comparison of different API design approaches.

## Features

- **Four Richardson Maturity Levels**: Complete implementations of Levels 0-3
- **Episode Data**: Information about all 9 seasons of "The Office"
- **Interactive Documentation**: Swagger UI for all API versions
- **Multiple Deployment Options**: Local, Docker, and Railway support
- **.NET 9.0**: Built with the latest .NET framework
- **Educational Resource**: Perfect for learning REST API design principles

## Richardson Maturity Model Levels

### Level 0: The Swamp of POX
- Single endpoint with all operations
- POST-only requests with action in payload
- RPC-style communication

### Level 1: Resources
- Multiple resource-based endpoints
- Still POST-only but URI represents resources
- Beginning of RESTful thinking

### Level 2: HTTP Verbs
- Proper use of HTTP methods (GET, POST, PUT, DELETE)
- Correct HTTP status codes
- Standard REST practices

### Level 3: HATEOAS
- Hypermedia as the Engine of Application State
- Responses include links to related resources
- Self-documenting API with discoverability

For detailed information about each level, see [Documentation/RichardsonMaturityModelOverview.md](Documentation/RichardsonMaturityModelOverview.md).

## Quick Start

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Docker (optional, for containerized deployment)

### Running Locally

1. **Clone the repository**
   ```bash
   git clone https://github.com/fszymaniak/TheOfficeAPI.git
   cd TheOfficeAPI
   ```

2. **Run the application**
   ```bash
   dotnet run --project src/TheOfficeAPI
   ```

3. **Access the API**
   - Application: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger
   - Health Check: http://localhost:5000/health

### Running with Docker

#### Using Docker Compose (Recommended)

```bash
# Build and run
docker-compose up -d --build

# View logs
docker-compose logs -f theofficeapi-level0

# Stop
docker-compose down
```

#### Using Docker directly

```bash
# Build the image
docker build -t theoffice-api:latest .

# Run the container
docker run -d \
  --name theoffice-api \
  -p 5000:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  theoffice-api:latest
```

Access the application at http://localhost:5000

## API Endpoints

All four API versions are available simultaneously through Swagger UI at `/swagger`.

### Level 0: Single Endpoint

```bash
POST /api/theOffice
Content-Type: application/json

{
  "action": "getAllSeasons"
}
```

### Level 1: Resource-Based URIs

```bash
POST /api/seasons
POST /api/seasons/{seasonNumber}/episodes
POST /api/seasons/{seasonNumber}/episodes/{episodeNumber}
```

### Level 2: HTTP Verbs

```bash
GET /api/seasons
GET /api/seasons/{seasonNumber}/episodes
GET /api/seasons/{seasonNumber}/episodes/{episodeNumber}
```

### Level 3: HATEOAS

```bash
GET /api/seasons
GET /api/seasons/{seasonNumber}/episodes
GET /api/episodes/{seasonNumber}/{episodeNumber}
```

Responses include hypermedia links to related resources:
```json
{
  "season": 2,
  "episodeNumber": 1,
  "title": "The Dundies",
  "releasedDate": "2005-09-20",
  "_links": {
    "self": {
      "href": "/api/episodes/2/1"
    },
    "season": {
      "href": "/api/seasons/2/episodes"
    },
    "allSeasons": {
      "href": "/api/seasons"
    }
  }
}
```

## Configuration

### Environment Variables

Configure which maturity level to run using environment variables:

```bash
# Run specific maturity level
export MATURITY_LEVEL=Level0  # Level0, Level1, Level2, or Level3
dotnet run --project src/TheOfficeAPI

# Or run all levels simultaneously (default)
dotnet run --project src/TheOfficeAPI
```

### Application Settings

Configuration is managed through `appsettings.json`:

```json
{
  "Server": {
    "DefaultUrl": "http://localhost:5000"
  },
  "Environment": {
    "MaturityLevelVariable": "MATURITY_LEVEL"
  }
}
```

## Project Structure

```
TheOfficeAPI/
├── src/
│   └── TheOfficeAPI/
│       ├── Level0/          # Richardson Level 0 implementation
│       ├── Level1/          # Richardson Level 1 implementation
│       ├── Level2/          # Richardson Level 2 implementation
│       ├── Level3/          # Richardson Level 3 implementation
│       ├── Common/          # Shared models and data
│       ├── Configuration/   # App configuration
│       └── Program.cs       # Application entry point
├── tests/                   # Test projects
├── Documentation/           # Additional documentation
├── scripts/                 # Utility scripts
├── Dockerfile              # Docker configuration
├── docker-compose.yaml     # Docker Compose configuration
└── README.md
```

## Development

### Build

```bash
dotnet build
```

### Test

```bash
dotnet test
```

### Mutation Testing

The project uses **Stryker.NET** for mutation testing to assess the quality and effectiveness of unit tests.

**View Live Mutation Report:** [https://fszymaniak.github.io/TheOfficeAPI/stryker/](https://fszymaniak.github.io/TheOfficeAPI/stryker/)

Mutation reports are automatically generated and published to GitHub Pages on every push to `main` or `develop` branches.

**Run Mutation Tests Locally:**
```bash
# Install Stryker tool (one-time setup)
dotnet tool restore

# Run mutation tests
dotnet stryker --config-file stryker-config.json

# Or use the convenient script
./scripts/run-mutation-tests.sh
```

**View Local Mutation Report:**
After running mutation tests, open `StrykerOutput/reports/mutation-report.html` in your browser to see:
- Mutation score (percentage of mutants killed)
- Survived mutants (potential test gaps)
- Detailed mutation coverage per file

**Understanding Mutation Testing:**
- **Mutants**: Small code changes that Stryker introduces
- **Killed**: Mutant causes tests to fail (good - tests caught the change)
- **Survived**: Mutant doesn't break tests (potential gap in test coverage)
- **Mutation Score**: Percentage of killed mutants (higher is better)

**Recommended Thresholds:**
- High: 80%+ (excellent test quality)
- Low: 60%+ (acceptable test quality)
- Below 60%: Consider improving tests

### Test Reports

The project provides comprehensive test reporting with both **Allure** test execution reports and **Stryker** mutation testing.

**📊 View Live Reports Dashboard:** [https://fszymaniak.github.io/TheOfficeAPI/](https://fszymaniak.github.io/TheOfficeAPI/)

All reports are automatically generated and published to GitHub Pages on every push to `main` or `develop` branches.

**Available Reports:**
- **[Allure Test Reports](https://fszymaniak.github.io/TheOfficeAPI/allure/)** - Rich, interactive test execution reports with history tracking
  - Unit Test Report - Tests for all API levels (Common, Level0-3)
  - Integration Test Report - Mocked integration tests
- **[Stryker Mutation Report](https://fszymaniak.github.io/TheOfficeAPI/stryker/)** - Mutation testing to assess test quality

**Local Report Generation:**
```bash
# Run tests with Allure
dotnet test -- xUnit.ReporterSwitch=allure

# Install Allure CLI (one-time)
brew install allure  # macOS
# or download from https://github.com/allure-framework/allure2/releases

# Generate report
allure generate ./tests/**/bin/**/allure-results -o allure-report --clean

# Open report in browser
allure open allure-report
```

### Run Specific Maturity Level

```bash
# Level 0
MATURITY_LEVEL=Level0 dotnet run --project src/TheOfficeAPI

# Level 1
MATURITY_LEVEL=Level1 dotnet run --project src/TheOfficeAPI

# Level 2
MATURITY_LEVEL=Level2 dotnet run --project src/TheOfficeAPI

# Level 3 (HATEOAS)
MATURITY_LEVEL=Level3 dotnet run --project src/TheOfficeAPI
```

## Deployment

### Mikrus VPS (mikrus.us)

Deploy to your own Mikrus VPS server with full control. This option provides:
- Complete server control and customization
- Cost-effective hosting (starting from ~10 PLN/month)
- Docker-based deployment with Nginx reverse proxy
- SSL/TLS support with Let's Encrypt
- Ideal for production deployments

**Quick Deploy:**
```bash
# On your Mikrus VPS
curl -fsSL https://raw.githubusercontent.com/fszymaniak/TheOfficeAPI/main/scripts/deploy-mikrus.sh | bash
```

For detailed instructions, see [Documentation/MikrusVPSDeployment.md](Documentation/MikrusVPSDeployment.md).

### Railway

The application is configured for deployment on Railway. It automatically:
- Detects the `PORT` environment variable
- Binds to `0.0.0.0` for external access
- Configures proper health checks

For deployment instructions, see [Documentation/RailwayDeployment.md](Documentation/RailwayDeployment.md).

### Docker Deployment

See [Documentation/DockerSetup.md](Documentation/DockerSetup.md) for detailed Docker deployment instructions.

## Learning Resources

This project is ideal for:
- Understanding RESTful API design principles
- Learning the Richardson Maturity Model
- Comparing different API architectural styles
- Teaching REST best practices

### Recommended Learning Path

1. Start with Level 0 to understand basic HTTP APIs
2. Move to Level 1 to learn resource-based design
3. Progress to Level 2 to master HTTP verbs and status codes
4. Finish with Level 3 to understand HATEOAS and hypermedia

## API Data

The API contains information about all 9 seasons of "The Office" including:
- Season numbers and episode counts
- Episode titles and release dates
- Episode numbers within seasons

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- **Richardson Maturity Model**: Leonard Richardson
- **The Office**: NBC Universal
- Built with .NET 9.0 and ASP.NET Core

## Contact

For questions or feedback, please open an issue on the GitHub repository.
