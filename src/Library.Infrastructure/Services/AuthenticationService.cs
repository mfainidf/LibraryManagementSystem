using System;
using System.Threading.Tasks;
using Library.Core.Models;
using Library.Core.Interfaces;
using Serilog;
using BC = BCrypt.Net.BCrypt;

namespace Library.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _logger = new LoggerConfiguration()
                .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter())
                .WriteTo.File(new Serilog.Formatting.Json.JsonFormatter(), 
                    "logs/auth.json",
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public async Task<User> RegisterUserAsync(string name, string email, string password)
        {
            _logger.Information("Starting user registration for {Email}", email);

            if (await _userRepository.EmailExistsAsync(email))
            {
                _logger.Warning("Registration failed: Email {Email} already exists", email);
                throw new InvalidOperationException("Email already exists");
            }

            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = BC.HashPassword(password),
                Role = UserRole.User,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow
            };

            try 
            {
                var createdUser = await _userRepository.CreateAsync(user);
                _logger.Information("User {Email} successfully registered with role {Role}", email, user.Role);
                return createdUser;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error registering user {Email}", email);
                throw;
            }
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            _logger.Information("Login attempt for user {Email}", email);
            
            var user = await _userRepository.GetByEmailAsync(email);
            
            if (user == null || !user.IsEnabled)
            {
                _logger.Warning("Login failed: User {Email} not found or disabled", email);
                throw new InvalidOperationException("Invalid credentials or account disabled");
            }

            if (!BC.Verify(password, user.PasswordHash))
            {
                _logger.Warning("Login failed: Invalid password for user {Email}", email);
                throw new InvalidOperationException("Invalid credentials");
            }

            _logger.Information("User {Email} successfully logged in", email);
            return user;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            _logger.Information("Password change attempt for user ID {UserId}", userId);
            
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                _logger.Warning("Password change failed: User ID {UserId} not found", userId);
                throw new InvalidOperationException("User not found");
            }

            if (!BC.Verify(oldPassword, user.PasswordHash))
            {
                _logger.Warning("Password change failed: Invalid current password for user ID {UserId}", userId);
                throw new InvalidOperationException("Invalid current password");
            }

            try
            {
                user.PasswordHash = BC.HashPassword(newPassword);
                await _userRepository.UpdateAsync(user);
                _logger.Information("Password successfully changed for user ID {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error changing password for user ID {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            _logger.Information("Password reset attempt for user ID {UserId}", userId);
            
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                _logger.Warning("Password reset failed: User ID {UserId} not found", userId);
                throw new InvalidOperationException("User not found");
            }

            user.PasswordHash = BC.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> SetUserRoleAsync(int userId, UserRole newRole)
        {
            _logger.Information("Attempting to change role to {NewRole} for user ID {UserId}", newRole, userId);
            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.Warning("Role change failed: User ID {UserId} not found", userId);
                throw new InvalidOperationException("User not found");
            }

            user.Role = newRole;
            await _userRepository.UpdateAsync(user);
            _logger.Information("Role successfully changed to {NewRole} for user ID {UserId}", newRole, userId);
            return true;
        }

        public async Task<User> RegisterAdminAsync(string name, string email, string password)
        {
            _logger.Information("Starting admin registration for {Email}", email);

            if (await _userRepository.EmailExistsAsync(email))
            {
                _logger.Warning("Admin registration failed: Email {Email} already exists", email);
                throw new InvalidOperationException("Email already exists");
            }

            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = BC.HashPassword(password),
                Role = UserRole.Administrator,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow
            };

            try 
            {
                var createdUser = await _userRepository.CreateAsync(user);
                _logger.Information("Admin user {Email} successfully registered", email);
                return createdUser;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error registering admin user {Email}", email);
                throw;
            }
        }
    }
}
