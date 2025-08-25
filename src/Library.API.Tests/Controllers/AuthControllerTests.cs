using System.IdentityModel.Tokens.Jwt;
using Sys            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<TokenResponse>(okResult.Value);
            Assert.NotNull(response.Token);

            var token = new JwtSecurityTokenHandler().ReadJwtToken(response.Token);urity.Claims;
using Library.API.Controllers;
using Library.API.Models;
using Library.Core.Interfaces;
using Library.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Library.API.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IConfigurationSection> _mockJwtSection;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockJwtSection = new Mock<IConfigurationSection>();

            // Setup JWT configuration
            _mockJwtSection.Setup(x => x["SecretKey"]).Returns("YourTestSecretKeyMustBeAtLeast32BytesLong!!");
            _mockJwtSection.Setup(x => x["Issuer"]).Returns("TestIssuer");
            _mockJwtSection.Setup(x => x["Audience"]).Returns("TestAudience");
            _mockJwtSection.Setup(x => x["ExpirationHours"]).Returns("24");

            _mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(_mockJwtSection.Object);

            _controller = new AuthController(_mockAuthService.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var request = new LoginRequest { Email = "test@test.com", Password = "password123" };
            var user = new User { Id = 1, Email = request.Email, Name = "Test User", Role = UserRole.User };
            _mockAuthService.Setup(x => x.LoginAsync(request.Email, request.Password)).ReturnsAsync(user);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value;
            string token = response.Token;
            Assert.NotNull(token);

            var parsedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            Assert.Equal(user.Id.ToString(), parsedToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Assert.Equal(user.Email, parsedToken.Claims.First(c => c.Type == ClaimTypes.Email).Value);
            Assert.Equal(user.Role.ToString(), parsedToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var request = new LoginRequest { Email = "test@test.com", Password = "wrongpassword" };
            _mockAuthService.Setup(x => x.LoginAsync(request.Email, request.Password))
                .ThrowsAsync(new InvalidOperationException("Invalid credentials"));

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(unauthorizedResult.Value);
            Assert.Equal("Invalid credentials", response.Message);
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsOkWithToken()
        {
            // Arrange
            var request = new RegisterRequest 
            { 
                Name = "Test User", 
                Email = "test@test.com", 
                Password = "password123" 
            };
            var user = new User { Id = 1, Name = request.Name, Email = request.Email, Role = UserRole.User };
            _mockAuthService.Setup(x => x.RegisterUserAsync(request.Name, request.Email, request.Password))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value;
            string token = response.Token;
            Assert.NotNull(token);

            var parsedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            Assert.Equal(user.Id.ToString(), parsedToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Assert.Equal(user.Email, parsedToken.Claims.First(c => c.Type == ClaimTypes.Email).Value);
            Assert.Equal(user.Role.ToString(), parsedToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        }

        [Fact]
        public async Task Register_WithExistingEmail_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterRequest 
            { 
                Name = "Test User", 
                Email = "existing@test.com", 
                Password = "password123" 
            };
            _mockAuthService.Setup(x => x.RegisterUserAsync(request.Name, request.Email, request.Password))
                .ThrowsAsync(new InvalidOperationException("Email already exists"));

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(badRequestResult.Value);
            Assert.Equal("Email already exists", response.Message);
        }

        [Fact]
        public async Task ChangePassword_WithValidData_ReturnsOk()
        {
            // Arrange
            var request = new ChangePasswordRequest 
            { 
                CurrentPassword = "oldPassword", 
                NewPassword = "newPassword" 
            };
            var userId = 1;
            var claims = new List<Claim> 
            { 
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()) 
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockAuthService.Setup(x => x.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ChangePassword(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(okResult.Value);
            Assert.Equal("Password changed successfully", response.Message);
        }

        [Fact]
        public async Task ChangePassword_WithInvalidCurrentPassword_ReturnsBadRequest()
        {
            // Arrange
            var request = new ChangePasswordRequest 
            { 
                CurrentPassword = "wrongPassword", 
                NewPassword = "newPassword" 
            };
            var userId = 1;
            var claims = new List<Claim> 
            { 
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()) 
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockAuthService.Setup(x => x.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword))
                .ThrowsAsync(new InvalidOperationException("Current password is incorrect"));

            // Act
            var result = await _controller.ChangePassword(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(badRequestResult.Value);
            Assert.Equal("Current password is incorrect", response.Message);
        }
    }


}
