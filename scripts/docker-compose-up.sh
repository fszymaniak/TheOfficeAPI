#!/bin/bash

# Start the Level0 application using Docker Compose
echo "Starting TheOfficeAPI Level0 with Docker Compose..."

docker-compose up -d --build

if [ $? -eq 0 ]; then
    echo "Level0 application started successfully!"
    echo "API is available at: http://localhost:5000"
    echo "Level0 endpoint: http://localhost:5000/api/theOffice"
    echo "Health check: http://localhost:5000/health"
    echo ""
    echo "Test the Level0 API:"
    echo "curl -X POST http://localhost:5000/api/theOffice \\"
    echo "  -H 'Content-Type: application/json' \\"
    echo "  -d '{\"action\": \"getAllSeasons\"}'"
    echo ""
    echo "To view logs: docker-compose logs -f theofficeapi-level0"
    echo "To stop: docker-compose down"
else
    echo "Failed to start application!"
    exit 1
fi