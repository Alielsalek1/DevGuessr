# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files for restore
COPY ["Backend/Domain/DevGuessr.Domain.csproj", "Backend/Domain/"]
COPY ["Backend/Application/DevGuessr.Application.csproj", "Backend/Application/"]
COPY ["Backend/Infrastructure/DevGuessr.Infrastructure.csproj", "Backend/Infrastructure/"]
COPY ["Backend/API/DevGuessr.API.csproj", "Backend/API/"]

# Restore dependencies for the API project only (ignores Tests)
RUN dotnet restore "Backend/API/DevGuessr.API.csproj"

# Copy entire source code
COPY . .

# Build the application
WORKDIR "/src/Backend/API"
RUN dotnet build "DevGuessr.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "DevGuessr.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

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

ENTRYPOINT ["dotnet", "DevGuessr.API.dll"]
