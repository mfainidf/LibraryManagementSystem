using System.Net;
using System.Net.Http.Json;
using Xunit;
using Library.Core.Models;
using Library.IntegrationTests;

namespace Library.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOkWithToken()
    {
        // Arrange
        var registerRequest = new
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Test123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("token", content.ToLower());
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var registerRequest = new
        {
            Name = "Login Test User",
            Email = "logintest@example.com",
            Password = "Test123!"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            Email = "logintest@example.com",
            Password = "Test123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("token", content.ToLower());
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new
        {
            Email = "nonexistent@example.com",
            Password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RegisterAdmin_WithoutAdminToken_ReturnsForbidden()
    {
        // Arrange
        var registerRequest = new
        {
            Name = "Admin Test",
            Email = "admintest@example.com",
            Password = "Admin123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register/admin", registerRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var changePasswordRequest = new
        {
            CurrentPassword = "OldPass123!",
            NewPassword = "NewPass123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/change-password", changePasswordRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
