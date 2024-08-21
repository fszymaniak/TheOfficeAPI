# Docker Setup for TheOfficeAPI Level0

This document explains how to run TheOfficeAPI Level0 using Docker.

## Prerequisites

- Docker installed on your machine
- Docker Compose (usually comes with Docker Desktop)

## Quick Start

### Option 1: Using Docker Compose (Recommended)

```bash
# Build and run the Level0 application
docker-compose up -d --build

# View logs
docker-compose logs -f theofficeapi-level0

# Stop the application
docker-compose down
```

### Option 2: Using Docker directly
# Build the image
docker build -t theoffice-api-level0:latest -f Dockerfile .

# Run the container
docker run -d \
--name theoffice-api-level0-container \
-p 5000:8080 \
-e ASPNETCORE_ENVIRONMENT=Development \
-e MATURITY_LEVEL=Level0 \
theoffice-api-level0:latest
