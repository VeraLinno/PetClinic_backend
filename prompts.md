# KiloCode Prompts for Pet Clinic Backend

## Main Prompt

[Insert the entire original task description here]

## Tier 1 Prompt

Generate the complete Tier 1 deliverables for the Pet Clinic backend:

1. Inspect the base repo Contact-Saas. Reuse its useful configuration, CI patterns, Docker-compose examples, and any frontend scaffold it contains. Merge or adapt that base into the new repository structure described below; preserve useful files from the base where appropriate (e.g., Docker/CI templates, environment patterns, README sections) and extend them to support the PetClinic backend and the Vue frontend dev stub.

2. Create solution PetClinic.sln and four projects with csproj files:
   - PetClinic.Domain — domain entities, enums.
   - PetClinic.Application — DTOs, interfaces, services, AutoMapper profiles.
   - PetClinic.Infrastructure — EF Core DbContext, repositories, migrations, refresh token entity.
   - PetClinic.Api — minimal hosting Program.cs, controllers, swagger, Authentication/Authorization registration.

3. Implement PetClinicDbContext with DbSets for Owner, Pet, Veterinarian, Appointment, Visit, Prescription, MedicationStock, Invoice, RefreshToken.

4. Add realistic OnModelCreating mapping (e.g., unique email, relations, enum mapping, conversion for Roles list).

5. Generate initial EF Core migration files (InitialCreate) in the Infrastructure Migrations folder. Include code for Up and Down.

6. Add seed data registration that runs on startup if DB empty: create one vet user (role = "Vet"), two owners (role = "Owner") and some pets, medication stock rows.

7. Create Dockerfile for Api and docker-compose.yml that includes db (postgres:15), api (built from Dockerfile), and frontend service that runs npm install && npm run dev from ./frontend (mount). Map ports so: api -> container 5000, exposed to host 5001 postgres -> 5432 frontend -> 5173

8. Create .env file at repo root containing at least:
   VITE_API_BASE_URL=http://localhost:5001/api/v1
   POSTGRES_USER=pguser
   POSTGRES_PASSWORD=pgpassword
   POSTGRES_DB=petclinicdb
   JWT_SECRET=ChangeThisInDevToAStrongOne

9. Create appsettings.Development.json sample for Api containing the connection string template and JWT secret.

10. Create root README.md with clear dev startup steps (docker-compose up, how to run migrations, how to run locally without docker). Also include a short section that documents what parts were reused/adapted from the base repo and why.

## Tier 2 Prompt

Implement the complete Tier 2 deliverables for the Pet Clinic backend:

1. Add necessary packages for JWT and auth (Microsoft.AspNetCore.Authentication.JwtBearer, System.IdentityModel.Tokens.Jwt, BCrypt.Net-Next).

2. Implement IUserContextService in Application layer with interface and Infrastructure implementation to extract current user id and roles from HttpContext.

3. Implement IAuthService in Application layer with AuthService in Infrastructure that:
   - Validates credentials using BCrypt.
   - Creates JWT access tokens with roles claim as array and subject claim = user id.
   - Issues refresh tokens: store hashed refresh token in DB tied to user with expiry and rotation support.
   - Implements refresh token rotation: when /auth/refresh is called with a valid (unrevoked) refresh token, create a new refresh token, persist it, revoke the old token, and return a new access token. Set refresh token as httpOnly cookie with secure flags.
   - Implements logout endpoint that revokes the refresh token and deletes cookie.

4. Implement AuthController endpoints:
   - POST /api/v1/auth/register (optional but include scaffold)
   - POST /api/v1/auth/login — returns { accessToken } and sets refreshToken httpOnly cookie.
   - POST /api/v1/auth/refresh — reads cookie, rotates tokens, returns new accessToken.
   - POST /api/v1/auth/logout — revoke refresh token.

5. Configure JWT bearer auth in Program.cs using Jwt:Secret and validate signing key.

6. Configure authorization policies for roles; usage of [Authorize(Roles="Vet")] in controllers where required.

7. Middleware & registration in Program.cs for authentication and authorization.

8. Ensure RefreshToken entity is properly configured in DbContext with relations.

## Tier 3 Prompt

Implement the complete Tier 3 deliverables for the Pet Clinic backend:

1. Add AutoMapper packages and create MappingProfile for DTO mappings.

2. Create DTOs in Application layer: OwnerDto, PetDto, AppointmentDto, CreateAppointmentDto, VisitDto, VisitCompletionDto, PrescriptionDto, InvoiceDto, MedicationStockDto.

3. Implement IAppointmentService and AppointmentService in Infrastructure with:
   - CreateAsync checking vet availability (no overlapping appointments: new.Start < existing.End && existing.Start < new.End).
   - Use EF Core transactions for concurrency prevention.
   - GetUserAppointmentsAsync scoped by user role (vet sees their appointments, owner sees pet appointments).

4. Implement IVisitService and VisitService with CompleteVisitAsync that:
   - Marks visit complete, creates invoice (sum services + medications), decrements MedicationStock quantities.
   - All inside a transaction with rollback on insufficient stock.
   - Throws meaningful errors for insufficient stock.

5. Implement OwnersController with:
   - GET /api/v1/owners/me (returns current owner profile).
   - GET /api/v1/owners/{id}/pets (with authorization checks).

6. Implement AppointmentsController with:
   - POST /api/v1/appointments (Owner role, creates appointment with availability check).
   - GET /api/v1/appointments (scoped by user role).

7. Implement VisitsController with:
   - PATCH /api/v1/visits/{id}/complete (Vet role only, completes visit with prescriptions).

8. Implement InventoryController with:
   - GET /api/v1/inventory/low-stock (Vet/Admin role, returns items with quantity < 10).

9. Register all services in Program.cs (AutoMapper, business services).

10. Ensure all controllers use proper authorization and return appropriate HTTP status codes.

## Tier 4 Prompt

Implement the complete Tier 4 deliverables for the Pet Clinic backend:

1. Create xUnit test project with references to all layers.

2. Write unit tests for AppointmentService:
   - Test availability logic with overlapping appointments (new.Start < existing.End && existing.Start < new.End).
   - Test successful appointment creation when vet is available.
   - Test exception when vet is not available.
   - Test exception when pet does not belong to user.

3. Write unit tests for VisitService:
   - Test CompleteVisitAsync with sufficient stock: verify invoice creation, stock decrement, prescription creation.
   - Test CompleteVisitAsync with insufficient stock: verify rollback and exception.
   - Test authorization checks for vet role.

4. Write integration tests using Testcontainers for PostgreSQL:
   - Test login endpoint with valid/invalid credentials.
   - Test health check endpoint.
   - Use WebApplicationFactory for API testing.

5. Create GitHub Actions CI workflow:
   - Build on push/PR to main/develop.
   - Matrix for .NET 8.0.
   - Restore, build, test.
   - Build Docker image and test health endpoint.

6. Add health check endpoint at /health.

7. Update README with test running instructions.