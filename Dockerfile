# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and all .csproj files
COPY *.sln ./
COPY src/TheOfficeAPI/*.csproj ./src/TheOfficeAPI/
COPY tests/TheOfficeAPI.Level0.Tests.Unit/*.csproj ./tests/TheOfficeAPI.Level0.Tests.Unit/
COPY tests/TheOfficeAPI.Level0.Tests.Integration/*.csproj ./tests/TheOfficeAPI.Level0.Tests.Integration/
COPY tests/TheOfficeAPI.Level1.Tests.Unit/*.csproj ./tests/TheOfficeAPI.Level1.Tests.Unit/
COPY tests/TheOfficeAPI.Level1.Tests.Integration/*.csproj ./tests/TheOfficeAPI.Level1.Tests.Integration/
COPY tests/TheOfficeAPI.Level2.Tests.Unit/*.csproj ./tests/TheOfficeAPI.Level2.Tests.Unit/
COPY tests/TheOfficeAPI.Level2.Tests.Integration/*.csproj ./tests/TheOfficeAPI.Level2.Tests.Integration/

# Restore dependencies
RUN dotnet restore TheOfficeAPI.sln

# Copy rest of the code
COPY . ./

# Publish main project
RUN dotnet publish src/TheOfficeAPI/TheOfficeAPI.csproj \
    -c Release \
    -o /app/out \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy compiled files
COPY --from=build /app/out .

# Set environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=8080

# Expose port 8080
EXPOSE 8080

# Entry point
ENTRYPOINT ["dotnet", "TheOfficeAPI.dll"]