# Database Migration
- dotnet ef migrations add AddTechnectionCategories --project Src/Infrastructure/Techdle.Infrastructure.csproj --startup-project Src/API/Techdle.API.csproj
- **why?** due to the clean architecture, the migrations are stored in the infrastructure project
# Database Update
- dotnet ef database update --project Src/Infrastructure/Techdle.Infrastructure.csproj  --startup-project Src/API/Techdle.API.csproj --connection <CONNECTION_STRING>
- **why?** because the dev enviornment differs from the docker env and obviously you need to have your DB running with docker