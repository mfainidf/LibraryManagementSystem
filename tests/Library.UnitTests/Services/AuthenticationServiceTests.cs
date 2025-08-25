using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Library.Core.Models;
using Library.Core.Interfaces;
using Library.Infrastructure.Services;

namespace Library.UnitTests.Services
{
    public class AuthenticationServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly AuthenticationService _authService;

        public AuthenticationServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _authService = new AuthenticationService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task RegisterUser_WithValidData_ShouldCreateUser()
        {
            // Arrange
            var name = "Test User";
            var email = "test@example.com";
            var password = "Password123!";

            _userRepositoryMock.Setup(x => x.EmailExistsAsync(email))
                .ReturnsAsync(false);

            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) => user);

            // Act
            var result = await _authService.RegisterUserAsync(name, email, password);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(name);
            result.Email.Should().Be(email);
            result.Role.Should().Be(UserRole.User);
            result.IsEnabled.Should().BeTrue();
            
            // Verify password is hashed
            result.PasswordHash.Should().NotBe(password);
            
            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterUser_WithExistingEmail_ShouldThrowException()
        {
            // Arrange
            var email = "existing@example.com";

            _userRepositoryMock.Setup(x => x.EmailExistsAsync(email))
                .ReturnsAsync(true);

            // Act & Assert
            await _authService.Invoking(x => x.RegisterUserAsync("Test", email, "password"))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Email already exists");
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnUser()
        {
            // Arrange
            var email = "test@example.com";
            var password = "Password123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = email,
                PasswordHash = hashedPassword,
                IsEnabled = true
            };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(email);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldThrowException()
        {
            // Arrange
            var email = "test@example.com";
            var password = "WrongPassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("RealPassword");

            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = email,
                PasswordHash = hashedPassword,
                IsEnabled = true
            };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act & Assert
            await _authService.Invoking(x => x.LoginAsync(email, password))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Invalid credentials");
        }

        [Fact]
        public async Task Login_WithDisabledAccount_ShouldThrowException()
        {
            // Arrange
            var email = "test@example.com";
            var password = "Password123!";

            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                IsEnabled = false
            };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act & Assert
            await _authService.Invoking(x => x.LoginAsync(email, password))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Invalid credentials or account disabled");
        }

        [Fact]
        public async Task ChangePassword_WithValidData_ShouldUpdatePassword()
        {
            // Arrange
            var userId = 1;
            var oldPassword = "OldPassword123!";
            var newPassword = "NewPassword123!";
            var hashedOldPassword = BCrypt.Net.BCrypt.HashPassword(oldPassword);

            var user = new User
            {
                Id = userId,
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = hashedOldPassword
            };

            User? capturedUser = null;
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .Returns(Task.FromResult(true));

            // Act
            var result = await _authService.ChangePasswordAsync(userId, oldPassword, newPassword);

            _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
            
            capturedUser.Should().NotBeNull();
            BCrypt.Net.BCrypt.Verify(newPassword, capturedUser!.PasswordHash).Should().BeTrue();
        }

        [Fact]
        public async Task ChangePassword_WithWrongOldPassword_ShouldThrowException()
        {
            // Arrange
            var userId = 1;
            var oldPassword = "WrongPassword";
            var newPassword = "NewPassword123!";
            var hashedRealPassword = BCrypt.Net.BCrypt.HashPassword("RealPassword");

            var user = new User
            {
                Id = userId,
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = hashedRealPassword
            };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act & Assert
            await _authService.Invoking(x => x.ChangePasswordAsync(userId, oldPassword, newPassword))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Invalid current password");
        }

        [Fact]
        public async Task SetUserRole_WithValidData_ShouldUpdateRole()
        {
            // Arrange
            var userId = 1;
            var newRole = UserRole.Administrator;

            var user = new User
            {
                Id = userId,
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "dummyhash",
                Role = UserRole.User
            };

            User? capturedUser = null;
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .Returns(Task.FromResult(true));

            // Act
            var result = await _authService.SetUserRoleAsync(userId, newRole);

            // Assert
            result.Should().BeTrue();
            capturedUser.Should().NotBeNull();
            capturedUser!.Role.Should().Be(UserRole.Administrator);
        }

        [Fact]
        public async Task SetUserRole_WithInvalidUserId_ShouldThrowException()
        {
            // Arrange
            var userId = 1;
            var newRole = UserRole.Administrator;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(default(User));

            // Act & Assert
            await _authService.Invoking(x => x.SetUserRoleAsync(userId, newRole))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("User not found");
        }

        [Fact]
        public async Task RegisterAdmin_WithValidData_ShouldCreateAdminUser()
        {
            // Arrange
            var name = "Admin User";
            var email = "admin@example.com";
            var password = "AdminPass123!";

            _userRepositoryMock.Setup(x => x.EmailExistsAsync(email))
                .ReturnsAsync(false);

            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) => user);

            // Act
            var result = await _authService.RegisterAdminAsync(name, email, password);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(name);
            result.Email.Should().Be(email);
            result.Role.Should().Be(UserRole.Administrator);
            result.IsEnabled.Should().BeTrue();
            
            // Verify password is hashed
            result.PasswordHash.Should().NotBe(password);
            
            _userRepositoryMock.Verify(x => x.CreateAsync(It.Is<User>(u => 
                u.Role == UserRole.Administrator)), Times.Once);
        }

        [Fact]
        public async Task RegisterAdmin_WithExistingEmail_ShouldThrowException()
        {
            // Arrange
            var name = "Admin User";
            var email = "admin@example.com";
            var password = "AdminPass123!";

            _userRepositoryMock.Setup(x => x.EmailExistsAsync(email))
                .ReturnsAsync(true);

            // Act & Assert
            await _authService.Invoking(x => x.RegisterAdminAsync(name, email, password))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Email already exists");
        }
    }
}
