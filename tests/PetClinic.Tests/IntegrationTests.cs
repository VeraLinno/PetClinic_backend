using System.Net;
using System.Net.Http.Json;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetClinic.Application;
using PetClinic.Domain;
using PetClinic.Infrastructure;
using Xunit;

namespace PetClinic.Tests;

public class IntegrationTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly PostgreSqlTestcontainer _postgresContainer;
    private HttpClient _client;

    public IntegrationTests()
    {
        _postgresContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "testdb",
                Username = "testuser",
                Password = "testpass"
            })
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
                        options.UseNpgsql(_postgresContainer.ConnectionString));
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
        await context.Database.MigrateAsync();

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
        var result = await response.Content.ReadFromJsonAsync<AuthResult>();
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.AccessToken.Should().NotBeNullOrEmpty();
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
    public async Task HealthCheck_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}