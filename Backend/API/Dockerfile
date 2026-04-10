# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files for restore
COPY ["Backend/Domain/Techdle.Domain.csproj", "Backend/Domain/"]
COPY ["Backend/Application/Techdle.Application.csproj", "Backend/Application/"]
COPY ["Backend/Infrastructure/Techdle.Infrastructure.csproj", "Backend/Infrastructure/"]
COPY ["Backend/API/Techdle.API.csproj", "Backend/API/"]

# Restore dependencies for the API project only (ignores Tests)
RUN dotnet restore "Backend/API/Techdle.API.csproj"

# Copy entire source code
COPY . .

# Build the application
WORKDIR "/src/Backend/API"
RUN dotnet build "Techdle.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Techdle.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Techdle.API.dll"]
