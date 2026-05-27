# Local Development

## PostgreSQL with Docker

This repository includes a Docker Compose setup for the local development database used by the backend.

### Start PostgreSQL

```sh
docker compose up -d postgres
```

The default database settings are:

- Host: `localhost`
- Port: `54320`
- Database: `coreproject_backend_dev`
- Username: `postgres`
- Password: `postgres`

To override these values, copy `.env.example` to `.env` and edit the values before starting Docker Compose.

### Apply EF Core migrations

From the repository root:

```sh
dotnet tool restore
```

```sh
dotnet ef database update \
  --project Backend/CoreProject.Backend.Infrastructure \
  --startup-project Backend/CoreProject.Backend.API
```

The EF Core design-time factory uses `NEWSTART_CONNECTION_STRING` when it is set. If the variable is not set, it falls back to the Docker development database connection string.

### Stop PostgreSQL

```sh
docker compose down
```

To remove the local database volume as well:

```sh
docker compose down -v
```
