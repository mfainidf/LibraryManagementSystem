using System.Threading.Tasks;
using Library.Core.Models;

namespace Library.Core.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User> RegisterUserAsync(string name, string email, string password);
        Task<User> LoginAsync(string email, string password);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<bool> ResetPasswordAsync(int userId, string newPassword);
        Task<bool> SetUserRoleAsync(int userId, UserRole newRole);
        Task<User> RegisterAdminAsync(string name, string email, string password);
    }
}
