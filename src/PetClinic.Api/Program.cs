using System.Text;
using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Serilog;
using PetClinic.Application;
using PetClinic.Infrastructure;

// Configure Serilog for centralized logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: Path.Combine("logs", "petclinic-.txt"),
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "PetClinic.Api")
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();  // Replace default logging with Serilog

    // Add services to the container.
    builder.Services.AddDbContext<PetClinicDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Auth services
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IUserContextService, UserContextService>();
    builder.Services.AddScoped<ILocalizationService, LocalizationService>();
    builder.Services.AddHttpContextAccessor();

    // Business services
    builder.Services.AddScoped<IAppointmentService, AppointmentService>();
    builder.Services.AddScoped<IVisitService, VisitService>();
    builder.Services.AddScoped<IAdminService, AdminService>();
    builder.Services.AddHostedService<InventoryDeliveryWorker>();

    // AutoMapper
    builder.Services.AddAutoMapper(typeof(MappingProfile));

    // API Versioning
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;  // Adds API-Version header to responses
    })
    .AddMvc();

    // JWT Authentication
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

    // Authorization policies
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Vet", policy => policy.RequireClaim("roles", "Vet"));
        options.AddPolicy("Owner", policy => policy.RequireClaim("roles", "Owner"));
        options.AddPolicy("Admin", policy => policy.RequireClaim("roles", "Admin"));
    });

    // CORS configuration - allows both local and public network origins
    var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
        ?? new[] { "http://localhost:5173", "http://localhost:3000" };

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecific", policy =>
        {
            policy.AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();

            if (builder.Environment.IsDevelopment())
            {
                policy.SetIsOriginAllowed(origin =>
                {
                    if (corsOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
                    {
                        return true;
                    }

                    if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                    {
                        return false;
                    }

                    // Allow HTTP LAN IPs in development, e.g. http://192.168.x.x:5173.
                    return uri.Scheme == Uri.UriSchemeHttp && IPAddress.TryParse(uri.Host, out _);
                });
            }
            else
            {
                policy.WithOrigins(corsOrigins);
            }
        });
    });

    // Rate Limiting configuration
    // Global: 100 requests per minute per user (or IP if not authenticated)
    // Auth endpoints: 5 attempts per minute per IP (stricter for security)
    builder.Services.AddRateLimiter(options =>
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.User.FindFirst("sub")?.Value
                    ?? context.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown",
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                }));

        // Stricter limiter for auth endpoints: 5 attempts per minute per IP
        options.AddPolicy("auth", context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                }));
    });

    var app = builder.Build();

    // Seed data
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<PetClinicDbContext>();

        // Wait for database to be ready
        var maxRetries = 10;
        var retryCount = 0;
        while (retryCount < maxRetries)
        {
            try
            {
                // Drop and recreate to ensure schema is up to date
                await dbContext.Database.EnsureDeletedAsync();
                await dbContext.Database.EnsureCreatedAsync();
                break;
            }
            catch
            {
                retryCount++;
                if (retryCount >= maxRetries)
                    throw;
                await Task.Delay(2000); // Wait 2 seconds before retry
            }
        }

        await SeedDataAsync(dbContext);
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Only redirect to HTTPS in production
    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseCors("AllowSpecific");
    app.UseRateLimiter();  // Rate limiting middleware (after CORS, before auth)
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

async Task SeedDataAsync(PetClinicDbContext context)
{
    if (await context.Owners.AnyAsync()) return; // Already seeded

    // Create vet
    var vet = new PetClinic.Domain.Owner
    {
        Email = "vet@petclinic.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), // Use proper hashing
        FirstName = "Dr. Sarah",
        LastName = "Johnson",
        Roles = new List<string> { "Vet" }
    };
    context.Owners.Add(vet);

    // Keep veterinarian profile ID aligned with authenticated vet owner ID.
    var veterinarian = new PetClinic.Domain.Veterinarian
    {
        Id = vet.Id,
        Name = "Sarah",
        LastName = "Johnson",
        Email = vet.Email,
        LicenseNumber = "VET-001"
    };
    context.Veterinarians.Add(veterinarian);

    // Create owners
    var owner1 = new PetClinic.Domain.Owner
    {
        Email = "owner1@petclinic.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
        FirstName = "John",
        LastName = "Smith",
        Roles = new List<string> { "Owner" }
    };
    var owner2 = new PetClinic.Domain.Owner
    {
        Email = "owner2@petclinic.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
        FirstName = "Alice",
        LastName = "Brown",
        Roles = new List<string> { "Owner" }
    };
    context.Owners.AddRange(owner1, owner2);

    // Create pets
    var pet1 = new PetClinic.Domain.Pet
    {
        Name = "Fluffy",
        Species = "Cat",
        Breed = "Persian",
        DateOfBirth = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc),
        OwnerId = owner1.Id
    };
    var pet2 = new PetClinic.Domain.Pet
    {
        Name = "Buddy",
        Species = "Dog",
        Breed = "Golden Retriever",
        DateOfBirth = new DateTime(2020, 7, 22, 0, 0, 0, DateTimeKind.Utc),
        OwnerId = owner2.Id
    };
    context.Pets.AddRange(pet1, pet2);

    // Create medication stock
    var med1 = new PetClinic.Domain.MedicationStock
    {
        Name = "Aspirin",
        Category = "Antibiotics",
        UnitPrice = 12.50m,
        Quantity = 100,
        Unit = "tablets",
        ReorderLevel = 30
    };
    var med2 = new PetClinic.Domain.MedicationStock
    {
        Name = "Ibuprofen",
        Category = "Antibiotics",
        UnitPrice = 9.75m,
        Quantity = 50,
        Unit = "tablets",
        ReorderLevel = 20
    };
    var med3 = new PetClinic.Domain.MedicationStock
    {
        Name = "Rabies Vaccine",
        Category = "Vaccines",
        UnitPrice = 47.00m,
        Quantity = 5,
        Unit = "doses",
        ReorderLevel = 10
    };
    var med4 = new PetClinic.Domain.MedicationStock
    {
        Name = "Amoxicillin",
        Category = "Antibiotics",
        UnitPrice = 18.25m,
        Quantity = 2,
        Unit = "bottles",
        ReorderLevel = 8
    };
    context.MedicationStocks.AddRange(med1, med2, med3, med4);

    await context.SaveChangesAsync();
}

public partial class Program { }
