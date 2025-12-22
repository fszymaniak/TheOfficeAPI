#!/bin/bash

# Mutation Testing Script for TheOfficeAPI using Stryker.NET
# This script runs mutation tests to assess the quality of unit tests

set -e

echo "========================================="
echo "   TheOfficeAPI Mutation Testing"
echo "========================================="
echo ""

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET SDK is not installed. Please install .NET 9.0 SDK."
    exit 1
fi

# Restore dotnet tools
echo -e "${YELLOW}Restoring dotnet tools...${NC}"
dotnet tool restore

# Run Stryker mutation tests
echo -e "${YELLOW}Running mutation tests with Stryker.NET...${NC}"
echo "This may take several minutes depending on the number of tests..."
echo ""

dotnet stryker --config-file stryker-config.json

# Check the exit code
if [ $? -eq 0 ]; then
    echo ""
    echo -e "${GREEN}Mutation testing completed successfully!${NC}"
    echo ""
    echo "View the HTML report at: StrykerOutput/reports/mutation-report.html"
    echo "View the JSON report at: StrykerOutput/reports/mutation-report.json"
else
    echo ""
    echo "Mutation testing failed or was interrupted."
    exit 1
fi
