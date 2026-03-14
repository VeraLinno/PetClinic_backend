# Pet Clinic Backend

This is the backend API for the Pet Clinic application, built with .NET 8, Clean Architecture, EF Core, and PostgreSQL.

## Architecture

The project follows Clean Architecture with the following layers:
- **Domain**: Entities and business rules
- **Application**: DTOs, interfaces, services
- **Infrastructure**: EF Core DbContext, repositories
- **Api**: Controllers, hosting, Swagger

## Technologies

- .NET 8
- Entity Framework Core with PostgreSQL
- JWT Authentication with refresh tokens
- Swagger/OpenAPI
- Docker & Docker Compose

## Development Setup

### Prerequisites

- .NET 8 SDK
- Docker & Docker Compose
- PostgreSQL (or use Docker)

### Running Locally

1. Clone the repository
2. Copy `.env` and update values if needed
3. Run with Docker Compose:
   ```bash
   docker-compose up --build
   ```
   This will start PostgreSQL and the API on http://localhost:5001

4. Alternatively, run locally:
   - Start PostgreSQL
   - Update `appsettings.Development.json` with connection string
   - Run migrations: `dotnet ef database update --project src/PetClinic.Infrastructure`
   - Run the API: `dotnet run --project src/PetClinic.Api`

### Running Tests

1. Unit tests:
   ```bash
   dotnet test tests/PetClinic.Tests/PetClinic.Tests.csproj --filter "Category=Unit"
   ```

2. Integration tests (requires Docker):
   ```bash
   dotnet test tests/PetClinic.Tests/PetClinic.Tests.csproj --filter "Category=Integration"
   ```

3. All tests:
   ```bash
   dotnet test
   ```

### API Documentation

Swagger UI is available at `http://localhost:5001/swagger` in development.

### Seed Data

The application seeds initial data on startup:
- 1 Vet user (vet@petclinic.com / password)
- 2 Owner users (owner1@petclinic.com, owner2@petclinic.com / password)
- Sample pets and medication stock

## Security Notes

- Refresh tokens are stored hashed in DB
- JWT access tokens include roles as array
- HttpOnly cookies for refresh tokens
- CORS configured for development

## Reused from Base Repo

The following were adapted from the base repository `https://git2.akaver.com/taltech-public/webapps-2025-spring/contact-saas.git`:
- `.gitignore` and `.dockerignore` patterns
- BaseEntity pattern for domain entities
- Project structure and naming conventions

## HTTP Examples

### Login
```bash
curl -X POST http://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"vet@petclinic.com","password":"password"}'
```

### Create Appointment
```bash
curl -X POST http://localhost:5001/api/v1/appointments \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"petId":"guid","veterinarianId":"guid","startAt":"2023-01-01T10:00:00Z","endAt":"2023-01-01T11:00:00Z"}'
```

### Complete Visit
```bash
curl -X PATCH http://localhost:5001/api/v1/visits/{visitId}/complete \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"notes":"Visit completed","prescriptions":[{"medication":"Aspirin","dosage":"100mg","quantity":10}]}'
