using System.Threading.Tasks;
using Library.Core.Models;

namespace Library.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> HasUserWithRoleAsync(UserRole role);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
