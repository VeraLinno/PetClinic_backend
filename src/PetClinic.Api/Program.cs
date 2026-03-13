using System.Text;
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

// CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PetClinicDbContext>();
    await dbContext.Database.MigrateAsync();
    await SeedDataAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
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
        Roles = new List<string> { "Vet" }
    };
    context.Owners.Add(vet);

    // Create owners
    var owner1 = new PetClinic.Domain.Owner
    {
        Email = "owner1@petclinic.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
        Roles = new List<string> { "Owner" }
    };
    var owner2 = new PetClinic.Domain.Owner
    {
        Email = "owner2@petclinic.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
        Roles = new List<string> { "Owner" }
    };
    context.Owners.AddRange(owner1, owner2);

    // Create pets
    var pet1 = new PetClinic.Domain.Pet
    {
        Name = "Fluffy",
        Species = "Cat",
        OwnerId = owner1.Id
    };
    var pet2 = new PetClinic.Domain.Pet
    {
        Name = "Buddy",
        Species = "Dog",
        OwnerId = owner2.Id
    };
    context.Pets.AddRange(pet1, pet2);

    // Create medication stock
    var med1 = new PetClinic.Domain.MedicationStock
    {
        Name = "Aspirin",
        Quantity = 100
    };
    var med2 = new PetClinic.Domain.MedicationStock
    {
        Name = "Ibuprofen",
        Quantity = 50
    };
    context.MedicationStocks.AddRange(med1, med2);

    await context.SaveChangesAsync();
}
