# Database Migration
- dotnet ef migrations add <Migration_Name> --project Backend/Infrastructure/DevGuessr.Infrastructure.csproj --startup-project Backend/API/DevGuessr.API.csproj
- **why?** due to the clean architecture, the migrations are stored in the infrastructure project
# Database Update
- dotnet ef database update --project Backend/Infrastructure/DevGuessr.Infrastructure.csproj  --startup-project Backend/API/DevGuessr.API.csproj --connection <CONNECTION_STRING>
- **why?** because the dev enviornment differs from the docker env and obviously you need to have your DB running with docker

