using System;
using System.Threading.Tasks;
using Library.Core.Models;
using Library.Core.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace Library.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> RegisterUserAsync(string name, string email, string password)
        {
            if (await _userRepository.EmailExistsAsync(email))
            {
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

            return await _userRepository.CreateAsync(user);
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            
            if (user == null || !user.IsEnabled)
            {
                throw new InvalidOperationException("Invalid credentials or account disabled");
            }

            if (!BC.Verify(password, user.PasswordHash))
            {
                throw new InvalidOperationException("Invalid credentials");
            }

            return user;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            if (!BC.Verify(oldPassword, user.PasswordHash))
            {
                throw new InvalidOperationException("Invalid current password");
            }

            user.PasswordHash = BC.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            user.PasswordHash = BC.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }
    }
}
