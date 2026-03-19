using System.Text;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetClinic.Application;
using PetClinic.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<PetClinicDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Auth services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor();

// Business services
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IVisitService, VisitService>();
builder.Services.AddHostedService<InventoryDeliveryWorker>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

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
        Quantity = 100,
        Unit = "tablets",
        ReorderLevel = 30
    };
    var med2 = new PetClinic.Domain.MedicationStock
    {
        Name = "Ibuprofen",
        Quantity = 50,
        Unit = "tablets",
        ReorderLevel = 20
    };
    var med3 = new PetClinic.Domain.MedicationStock
    {
        Name = "Rabies Vaccine",
        Quantity = 5,
        Unit = "doses",
        ReorderLevel = 10
    };
    var med4 = new PetClinic.Domain.MedicationStock
    {
        Name = "Amoxicillin",
        Quantity = 2,
        Unit = "bottles",
        ReorderLevel = 8
    };
    context.MedicationStocks.AddRange(med1, med2, med3, med4);

    await context.SaveChangesAsync();
}
