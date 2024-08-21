#!/bin/bash

# Run the Docker container for Level0 API
echo "Starting TheOfficeAPI Level0 container..."

docker run -d \
    --name theoffice-api-level0-container \
    -p 5000:8080 \
    -e ASPNETCORE_ENVIRONMENT=Development \
    -e MATURITY_LEVEL=Level0 \
    theoffice-api-level0:latest

if [ $? -eq 0 ]; then
    echo "Level0 container started successfully!"
    echo "API is available at: http://localhost:5000"
    echo "Level0 endpoint: http://localhost:5000/api/theOffice"
    echo "Health check: http://localhost:5000/health"
    echo ""
    echo "Test the Level0 API:"
    echo "curl -X POST http://localhost:5000/api/theOffice \\"
    echo "  -H 'Content-Type: application/json' \\"
    echo "  -d '{\"action\": \"getAllSeasons\"}'"
    echo ""
    echo "To view logs: docker logs theoffice-api-level0-container"
    echo "To stop: docker stop theoffice-api-level0-container"
    echo "To remove: docker rm theoffice-api-level0-container"
else
    echo "Failed to start container!"
    exit 1
fi