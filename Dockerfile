# Use the official .NET 9.0 runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution file and all project files
COPY ["TheOfficeAPI.sln", "./"]
COPY ["src/TheOfficeAPI/TheOfficeAPI.csproj", "src/TheOfficeAPI/"]
COPY ["tests/TheOfficeAPI.Level0.Tests.Unit/TheOfficeAPI.Level0.Tests.Unit.csproj", "tests/TheOfficeAPI.Level0.Tests.Unit/"]
COPY ["tests/TheOfficeAPI.Level0.Tests.Integration/TheOfficeAPI.Level0.Tests.Integration.csproj", "tests/TheOfficeAPI.Level0.Tests.Integration/"]

# Restore dependencies for the entire solution
RUN dotnet restore "TheOfficeAPI.sln"

# Copy everything else and build
COPY . .
WORKDIR "/src"

# Build the main API project
RUN dotnet build "src/TheOfficeAPI/TheOfficeAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "src/TheOfficeAPI/TheOfficeAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables specifically for Level0
ENV ASPNETCORE_ENVIRONMENT=Production
ENV MATURITY_LEVEL=Level0
ENV ASPNETCORE_URLS=http://+:8080

# Ensure Level0 services are loaded
ENV DOTNET_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "TheOfficeAPI.dll"]