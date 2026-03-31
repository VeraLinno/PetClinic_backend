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
    builder.Services.AddScoped<ITranslationService, TranslationService>();
    builder.Services.AddHostedService<InventoryDeliveryWorker>();

    // AutoMapper
    builder.Services.AddAutoMapper(typeof(MappingProfile));

    // API Versioning - URL based (e.g., /api/v1/appointments)
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;  // Adds API-Version header to responses
        options.ApiVersionReader = new UrlSegmentApiVersionReader();  // Read version from URL
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
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<TranslationService>();

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

        await SeedDataAsync(dbContext, logger);
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

async Task SeedDataAsync(PetClinicDbContext context, ILogger<TranslationService> logger)
{
    var ownerCount = await context.Owners.CountAsync();
    var translationCount = await context.Translations.CountAsync();

    if (ownerCount > 0 && translationCount > 0) return; // Already seeded

    if (ownerCount == 0)
    {
        // Create admin user
        var admin = new PetClinic.Domain.Owner
        {
            Email = "admin@petclinic.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            FirstName = "Admin",
            LastName = "User",
            Roles = new List<string> { "Admin" }
        };
        context.Owners.Add(admin);

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

    // Seed translations
    if (translationCount == 0)
    {
        var translationService = new TranslationService(context, logger);

        // Load translations from JSON files
        var translationsData = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        // English translations
        translationsData["en"] = new()
        {
            ["common"] = new() { ["ok"] = "OK", ["cancel"] = "Cancel", ["save"] = "Save", ["delete"] = "Delete", ["edit"] = "Edit", ["close"] = "Close", ["loading"] = "Loading...", ["error"] = "Error", ["success"] = "Success", ["warning"] = "Warning", ["info"] = "Information", ["back"] = "Back", ["next"] = "Next", ["previous"] = "Previous", ["search"] = "Search", ["filter"] = "Filter", ["logout"] = "Logout", ["login"] = "Login", ["register"] = "Register", ["yes"] = "Yes", ["no"] = "No", ["confirm"] = "Confirm", ["settings"] = "Settings", ["profile"] = "Profile", ["dashboard"] = "Dashboard", ["home"] = "Home" },
            ["auth"] = new() { ["loginPortalTitle"] = "Pet Clinic", ["email"] = "Email", ["password"] = "Password", ["firstName"] = "First Name", ["lastName"] = "Last Name", ["phoneNumber"] = "Phone Number", ["registerAsOwner"] = "Register as Pet Owner", ["registerAsVet"] = "Register as Veterinarian", ["selectRole"] = "Select your role", ["petOwner"] = "Pet Owner", ["veterinarian"] = "Veterinarian", ["haveAccount"] = "Already have an account?", ["noAccount"] = "Don't have an account?", ["loginHere"] = "Login here", ["registerHere"] = "Register here", ["licenseNumber"] = "License Number", ["specialization"] = "Specialization", ["invalidCredentials"] = "Invalid email or password", ["emailTaken"] = "Email already in use", ["registrationSuccess"] = "Registration successful! You can now login.", ["loginSuccess"] = "Login successful!", ["logoutSuccess"] = "You have been logged out." },
            ["appointments"] = new() { ["title"] = "Appointments", ["bookNew"] = "Book New Appointment", ["myAppointments"] = "My Appointments", ["selectPet"] = "Select Pet", ["selectVet"] = "Select Veterinarian", ["selectDate"] = "Select Date", ["selectTime"] = "Select Time", ["reason"] = "Reason for Visit", ["notes"] = "Additional Notes", ["scheduled"] = "Appointment scheduled successfully", ["cancelled"] = "Appointment cancelled", ["confirmCancel"] = "Are you sure you want to cancel this appointment?", ["date"] = "Date", ["time"] = "Time", ["duration"] = "Duration", ["status"] = "Status", ["status_scheduled"] = "Scheduled", ["status_completed"] = "Completed", ["status_cancelled"] = "Cancelled", ["veterinarian"] = "Veterinarian" }
        };

        // Estonian translations
        translationsData["et"] = new()
        {
            ["common"] = new() { ["ok"] = "OK", ["cancel"] = "Tühista", ["save"] = "Salvesta", ["delete"] = "Kustuta", ["edit"] = "Muuda", ["close"] = "Sulge", ["loading"] = "Laadimises...", ["error"] = "Viga", ["success"] = "Edukalt", ["warning"] = "Hoiatus", ["info"] = "Teave", ["back"] = "Tagasi", ["next"] = "Järgmine", ["previous"] = "Eelmine", ["search"] = "Otsi", ["filter"] = "Filtreeri", ["logout"] = "Logi välja", ["login"] = "Logi sisse", ["register"] = "Registreeri", ["yes"] = "Jah", ["no"] = "Ei", ["confirm"] = "Kinnita", ["settings"] = "Seadistused", ["profile"] = "Profiil", ["dashboard"] = "Juhtpaneel", ["home"] = "Avaleht" },
            ["auth"] = new() { ["email"] = "E-post", ["password"] = "Parool", ["firstName"] = "Eesnimi", ["lastName"] = "Perekonnanimi", ["phoneNumber"] = "Telefoni number", ["registerAsOwner"] = "Registreeri lemmikloomade omanikuna", ["registerAsVet"] = "Registreeri veterinaarspetsialistina", ["selectRole"] = "Valige oma roll", ["petOwner"] = "Lemmikloomade omanik", ["veterinarian"] = "Veterinaarspetsialist", ["haveAccount"] = "Mul on juba konto?", ["noAccount"] = "Teil pole kontot?", ["loginHere"] = "Logige sisse siin", ["registerHere"] = "Registreeruge siin", ["licenseNumber"] = "Litsentsid number", ["specialization"] = "Spetsialiseerumine", ["invalidCredentials"] = "Vale e-post või parool", ["emailTaken"] = "E-posti aadress on juba kasutusel", ["registrationSuccess"] = "Registreerimine oli edukas! Nüüd saate sisse logida.", ["loginSuccess"] = "Sisselogimine oli edukas!", ["logoutSuccess"] = "Olete välja logitud." },
            ["appointments"] = new() { ["title"] = "Aja saadiused", ["bookNew"] = "Broneerida uus aeg", ["myAppointments"] = "Minu aja saadiused", ["selectPet"] = "Valige lemmikloom", ["selectVet"] = "Valige veterinaarspetsialist", ["selectDate"] = "Valige kuupäev", ["selectTime"] = "Valige aeg", ["reason"] = "Külastuse põhjus", ["notes"] = "Lisateave", ["scheduled"] = "Aeg broneeritud edukalt", ["cancelled"] = "Aeg tühistatud", ["confirmCancel"] = "Olete kindel, et soovite seda aega tühistada?", ["date"] = "Kuupäev", ["time"] = "Aeg", ["duration"] = "Kestus", ["status"] = "Olek", ["status_scheduled"] = "Planeeritud", ["status_completed"] = "Lõpetatud", ["status_cancelled"] = "Tühistatud", ["veterinarian"] = "Veterinaarspetsialist" }
        };

        // Russian translations
        translationsData["ru"] = new()
        {
            ["common"] = new() { ["ok"] = "ОК", ["cancel"] = "Отмена", ["save"] = "Сохранить", ["delete"] = "Удалить", ["edit"] = "Редактировать", ["close"] = "Закрыть", ["loading"] = "Загрузка...", ["error"] = "Ошибка", ["success"] = "Успешно", ["warning"] = "Предупреждение", ["info"] = "Информация", ["back"] = "Назад", ["next"] = "Далее", ["previous"] = "Назад", ["search"] = "Поиск", ["filter"] = "Фильтр", ["logout"] = "Выход", ["login"] = "Вход", ["register"] = "Регистрация", ["yes"] = "Да", ["no"] = "Нет", ["confirm"] = "Подтвердить", ["settings"] = "Настройки", ["profile"] = "Профиль", ["dashboard"] = "Панель управления", ["home"] = "Главная" },
            ["auth"] = new() { ["email"] = "Электронная почта", ["password"] = "Пароль", ["firstName"] = "Имя", ["lastName"] = "Фамилия", ["phoneNumber"] = "Номер телефона", ["registerAsOwner"] = "Зарегистрироваться как владелец питомца", ["registerAsVet"] = "Зарегистрироваться как ветеринар", ["selectRole"] = "Выберите вашу роль", ["petOwner"] = "Владелец питомца", ["veterinarian"] = "Ветеринар", ["haveAccount"] = "Уже есть аккаунт?", ["noAccount"] = "Нет аккаунта?", ["loginHere"] = "Войдите здесь", ["registerHere"] = "Зарегистрируйтесь здесь", ["licenseNumber"] = "Номер лицензии", ["specialization"] = "Специализация", ["invalidCredentials"] = "Неверный адрес электронной почты или пароль", ["emailTaken"] = "Этот адрес электронной почты уже используется", ["registrationSuccess"] = "Регистрация прошла успешно! Теперь вы можете войти.", ["loginSuccess"] = "Вход выполнен успешно!", ["logoutSuccess"] = "Вы вышли из системы." },
            ["appointments"] = new() { ["title"] = "Записи", ["bookNew"] = "Записаться на новый прием", ["myAppointments"] = "Мои записи", ["selectPet"] = "Выберите питомца", ["selectVet"] = "Выберите ветеринара", ["selectDate"] = "Выберите дату", ["selectTime"] = "Выберите время", ["reason"] = "Причина визита", ["notes"] = "Дополнительные заметки", ["scheduled"] = "Запись успешно создана", ["cancelled"] = "Запись отменена", ["confirmCancel"] = "Вы уверены, что хотите отменить эту запись?", ["date"] = "Дата", ["time"] = "Время", ["duration"] = "Длительность", ["status"] = "Статус", ["status_scheduled"] = "Запланировано", ["status_completed"] = "Завершено", ["status_cancelled"] = "Отменено", ["veterinarian"] = "Ветеринар" }
        };

        await translationService.SeedTranslationsAsync(translationsData);
    }
}


public partial class Program { }
