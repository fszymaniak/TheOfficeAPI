# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and all .csproj files for better caching
COPY *.sln ./
COPY src/TheOfficeAPI/*.csproj ./src/TheOfficeAPI/
COPY tests/TheOfficeAPI.Level0.Tests.Unit/*.csproj ./tests/TheOfficeAPI.Level0.Tests.Unit/
COPY tests/TheOfficeAPI.Level0.Tests.Integration/*.csproj ./tests/TheOfficeAPI.Level0.Tests.Integration/

# Restore dependencies (cached if .csproj files don't change)
RUN dotnet restore TheOfficeAPI.sln

# Copy rest of the code
COPY . ./

# Publish main project only (faster than entire solution)
RUN dotnet publish src/TheOfficeAPI/TheOfficeAPI.csproj \
    -c Release \
    -o /app/out \
    --no-restore

# Runtime stage (smaller image)
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy only compiled files
COPY --from=build /app/out .

# Railway automatically sets PORT environment variable
# Using ${PORT} without default value, Railway always sets it
ENV ASPNETCORE_URLS=http://+:${PORT}
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose for documentation (Railway ignores this and uses its own PORT)
EXPOSE 8080

# Entry point
ENTRYPOINT ["dotnet", "TheOfficeAPI.dll"]