using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetClinic.Application;
using PetClinic.Domain;
using PetClinic.Infrastructure;
using Testcontainers.PostgreSql;
using Xunit;

namespace PetClinic.Tests;

public class IntegrationTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly PostgreSqlContainer _postgresContainer;
    private HttpClient _client = default!;

    public IntegrationTests()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .Build();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PetClinicDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<PetClinicDbContext>(options =>
                        options.UseNpgsql(_postgresContainer.GetConnectionString()));

                    // Ensure schema exists before Program.cs startup seeding executes.
                    using var serviceProvider = services.BuildServiceProvider();
                    using var scope = serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<PetClinicDbContext>();
                    context.Database.EnsureCreated();
                });
            });
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        _client = _factory.CreateClient();

        // Seed test data
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PetClinicDbContext>();
        await context.Database.EnsureCreatedAsync();

        var vet = new Owner
        {
            Email = "vet@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            Roles = new List<string> { "Vet" }
        };
        context.Owners.Add(vet);
        await context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        _factory.Dispose();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Login_ShouldReturnAccessToken_WhenCredentialsAreValid()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "vet@test.com",
            Password = "password"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.TryGetProperty("accessToken", out var accessToken).Should().BeTrue();
        accessToken.GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "vet@test.com",
            Password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Login_ShouldReturnUnauthorized_WhenEmailContainsSqlInjectionPayload()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "' OR 1=1 --",
            Password = "password"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordContainsSqlInjectionPayload()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "vet@test.com",
            Password = "' OR '1'='1"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task HealthCheck_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Owner_ShouldCreateAndListOwnPet()
    {
        var email = $"owner-{Guid.NewGuid():N}@test.com";
        var password = "Password123!";

        await RegisterAndLoginAsync(email, password);

        var createPetResponse = await _client.PostAsJsonAsync("/api/v1/owners/me/pets", new CreatePetDto
        {
            Name = "Milo",
            Species = "Cat",
            Breed = "Tabby",
            DateOfBirth = DateTime.UtcNow.AddYears(-2)
        });

        createPetResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var petsResponse = await _client.GetAsync("/api/v1/owners/me/pets");
        petsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var pets = await petsResponse.Content.ReadFromJsonAsync<List<PetDto>>();
        pets.Should().NotBeNull();
        pets!.Should().ContainSingle(p => p.Name == "Milo");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Owner_ShouldNotDeleteOtherOwnersPet_IdorProtection()
    {
        var owner1Email = $"owner1-{Guid.NewGuid():N}@test.com";
        var owner2Email = $"owner2-{Guid.NewGuid():N}@test.com";
        var password = "Password123!";

        await RegisterAndLoginAsync(owner2Email, password);

        var createPetResponse = await _client.PostAsJsonAsync("/api/v1/owners/me/pets", new CreatePetDto
        {
            Name = "Luna",
            Species = "Dog",
            Breed = "Mixed",
            DateOfBirth = DateTime.UtcNow.AddYears(-1)
        });
        createPetResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdPet = await createPetResponse.Content.ReadFromJsonAsync<PetDto>();
        createdPet.Should().NotBeNull();

        await RegisterAndLoginAsync(owner1Email, password);

        var deleteResponse = await _client.DeleteAsync($"/api/v1/owners/me/pets/{createdPet!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task RegisterAndLoginAsync(string email, string password)
    {
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            FirstName = "Test",
            LastName = "User"
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest
        {
            Email = email,
            Password = password
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginJson = await loginResponse.Content.ReadAsStringAsync();
        using var loginDoc = JsonDocument.Parse(loginJson);
        var accessToken = loginDoc.RootElement.GetProperty("accessToken").GetString();

        accessToken.Should().NotBeNullOrWhiteSpace();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
}