using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Library.Core.Models;
using Library.Infrastructure.Data;
using Library.Infrastructure.Repositories;

namespace Library.IntegrationTests.Repositories
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly LibraryDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _context = new LibraryDbContext(options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();
            
            _repository = new UserRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.CloseConnection();
            _context.Dispose();
        }

        [Fact]
        public async Task CreateUser_ShouldPersistUser()
        {
            // Arrange
            var user = new User
            {
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Role = UserRole.User,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = await _repository.CreateAsync(user);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);

            var savedUser = await _context.Users.FindAsync(result.Id);
            savedUser.Should().NotBeNull();
            savedUser.Email.Should().Be(user.Email);
            savedUser.Name.Should().Be(user.Name);
        }

        [Fact]
        public async Task GetByEmail_WithExistingEmail_ShouldReturnUser()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User
            {
                Name = "Test User",
                Email = email,
                PasswordHash = "hashedpassword",
                Role = UserRole.User,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByEmailAsync(email);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(email);
        }

        [Fact]
        public async Task EmailExists_WithExistingEmail_ShouldReturnTrue()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User
            {
                Name = "Test User",
                Email = email,
                PasswordHash = "hashedpassword",
                Role = UserRole.User,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.EmailExistsAsync(email);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateUser_ShouldPersistChanges()
        {
            // Arrange
            var user = new User
            {
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Role = UserRole.User,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            user.Name = "Updated Name";
            await _repository.UpdateAsync(user);

            // Assert
            var updatedUser = await _context.Users.FindAsync(user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser.Name.Should().Be("Updated Name");
        }
    }
}
