# Development Database Setup

## 1. Default Approach
- Primary development database: `Local PostgreSQL`
- Fallback option: `Docker PostgreSQL`
- Current backend development database:
  - `Host=localhost`
  - `Port=5432`
  - `Database=coreproject_backend_dev`

## 2. Local PostgreSQL

### Current verified local environment
- PostgreSQL version:
  - `18.4`
- Installed location:
  - `E:\SQL\Postgresql\18`
- Verified server:
  - `localhost:5432`

### Current connection used for local verification
- `Host=localhost;Port=5432;Database=coreproject_backend_dev;Username=postgres;Password=p@$$w0rd`

### Backend development config
- File:
  - [Backend/CoreProject.Backend.API/appsettings.Development.json](E:\Project\NewStart\Backend\CoreProject.Backend.API\appsettings.Development.json)

### Verified result
- Database created:
  - `coreproject_backend_dev`
- EF migration applied:
  - `20260520200331_InitialCreate`
- Verified tables:
  - `__EFMigrationsHistory`
  - `configuration_entries`

## 3. Commands Used

### Check PostgreSQL version
```powershell
& 'E:\SQL\Postgresql\18\bin\psql.exe' --version
```

### Connect and verify server
```powershell
$env:PGPASSWORD='p@$$w0rd'
& 'E:\SQL\Postgresql\18\bin\psql.exe' -h localhost -p 5432 -U postgres -d postgres -c "select version();"
```

### Create development database
```powershell
$env:PGPASSWORD='p@$$w0rd'
& 'E:\SQL\Postgresql\18\bin\psql.exe' -h localhost -p 5432 -U postgres -d postgres -c "CREATE DATABASE coreproject_backend_dev;"
```

### Apply EF migration
```powershell
$env:ASPNETCORE_ENVIRONMENT='Development'
$env:ConnectionStrings__DefaultConnection='Host=localhost;Port=5432;Database=coreproject_backend_dev;Username=postgres;Password=p@$$w0rd'
dotnet ef database update --project Backend\CoreProject.Backend.Infrastructure\CoreProject.Backend.Infrastructure.csproj --startup-project Backend\CoreProject.Backend.API\CoreProject.Backend.API.csproj --no-build
```

## 4. API Verification

### Start API
```powershell
$env:ASPNETCORE_ENVIRONMENT='Development'
dotnet run --no-build --project Backend\CoreProject.Backend.API\CoreProject.Backend.API.csproj --launch-profile http
```

### Verify health
```powershell
curl.exe -i http://localhost:5046/health
```

### Verify sample endpoint
```powershell
curl.exe -i http://localhost:5046/api/system/info
```

## 5. Docker Fallback

### When to use Docker
- local PostgreSQL is not installed
- another machine needs a repeatable dev database quickly
- local credentials are unknown or unavailable

### Example Docker run
```powershell
docker run --name newstart-postgres ^
  -e POSTGRES_USER=postgres ^
  -e POSTGRES_PASSWORD=p@$$w0rd ^
  -e POSTGRES_DB=coreproject_backend_dev ^
  -p 5432:5432 ^
  -d postgres:18
```

### Example connection string for Docker fallback
- `Host=localhost;Port=5432;Database=coreproject_backend_dev;Username=postgres;Password=p@$$w0rd`

## 6. Important Notes
- For EF tooling in this project, `ConnectionStrings__DefaultConnection` environment override is the most reliable option during `dotnet ef` execution.
- `DesignTimeDbContextFactory` was adjusted so tooling can better resolve development connection settings.
- Local HTTP verification was confirmed on:
  - `http://localhost:5046/health`
  - `http://localhost:5046/api/system/info`
- Integration tests in this environment still hit a Windows Event Log permission issue unrelated to PostgreSQL connectivity.

