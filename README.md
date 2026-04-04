# Pet Clinic Backend

Backend API for the Pet Clinic application, built with .NET 9, Clean Architecture, EF Core, and PostgreSQL.

## Stack

- .NET 9 (`net9.0`)
- ASP.NET Core Web API
- Entity Framework Core + PostgreSQL
- JWT authentication + refresh token flow
- API versioning (`/api/v{version}`)
- Serilog file + console logging
- Docker / Docker Compose

## Project Structure

- `src/PetClinic.Domain` - entities and domain rules
- `src/PetClinic.Application` - DTOs, interfaces, service contracts
- `src/PetClinic.Infrastructure` - EF Core, persistence, service implementations
- `src/PetClinic.Api` - controllers, middleware, hosting
- `tests/PetClinic.Tests` - unit and integration tests

## Prerequisites

- .NET SDK 9.0.x (see `global.json`)
- Docker Desktop (recommended)
- PostgreSQL 16+ (only if not using Docker)

## Quick Start (Docker)

From this folder (`velinn-petclinic_charp`):

```bash
docker compose up --build
```

Services started by Compose:

- API: `http://localhost:5001`
- PostgreSQL: `localhost:5432`
- Frontend (if built from the sibling frontend folder): `http://localhost`

Stop services:

```bash
docker compose down
```

## Run Backend Without Docker

1. Start PostgreSQL.
2. Update connection string in `src/PetClinic.Api/appsettings.Development.json`.
3. Run the API:

```bash
dotnet run --project src/PetClinic.Api
```

Default local launch profile URL:

- `http://localhost:5023`

The app applies migrations automatically on startup.

## Environment Variables (Common)

- `ASPNETCORE_ENVIRONMENT` (default in Compose: `Production`)
- `ASPNETCORE_URLS` (Compose default: `http://+:5000`)
- `JWT_SECRET` (mapped to `Jwt__Secret`)
- `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`

## Tests

Run all tests:

```bash
dotnet test
```

Run unit tests only:

```bash
dotnet test tests/PetClinic.Tests/PetClinic.Tests.csproj --filter "Category=Unit"
```

Run integration tests only:

```bash
dotnet test tests/PetClinic.Tests/PetClinic.Tests.csproj --filter "Category=Integration"
```

## API Docs

Swagger UI is enabled in Development:

- `http://localhost:5023/swagger` (local dotnet run)
- `http://localhost:5001/swagger` (when running with Compose)

## Seed Data

Initial data is seeded on startup, including:

- Vet user: `vet@petclinic.com` / `password`
- Owner users: `owner1@petclinic.com`, `owner2@petclinic.com` / `password`
- Sample pets, appointments, and inventory data

## Security Notes

- Refresh tokens are stored hashed in the database.
- Access tokens contain role claims used by API authorization policies.
- Refresh flow uses HttpOnly cookies.
- CORS allows configured local origins and LAN IP origins in development.

## Example Request

```bash
curl -X POST http://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"vet@petclinic.com","password":"password"}'
```
