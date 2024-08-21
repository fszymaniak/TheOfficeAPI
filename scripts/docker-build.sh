#!/bin/bash

# Build the Docker image for Level0 API
echo "Building TheOfficeAPI Level0 Docker image..."
docker build -t theoffice-api-level0:latest -f Dockerfile .

if [ $? -eq 0 ]; then
    echo "Docker image built successfully!"
    echo "Image: theoffice-api-level0:latest"
    echo "Configured for Richardson Maturity Level 0"
    docker images theoffice-api-level0:latest
else
    echo "Docker build failed!"
    exit 1
fi