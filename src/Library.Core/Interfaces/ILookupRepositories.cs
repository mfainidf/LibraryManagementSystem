using System.Collections.Generic;
using System.Threading.Tasks;
using Library.Core.Models;

namespace Library.Core.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> CreateAsync(Category category);
        Task<Category?> GetByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<IEnumerable<Category>> GetActiveAsync();
        Task<bool> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);
        
        Task<Category?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Category>> SearchAsync(string searchTerm);
    }

    public interface IGenreRepository
    {
        Task<Genre> CreateAsync(Genre genre);
        Task<Genre?> GetByIdAsync(int id);
        Task<IEnumerable<Genre>> GetAllAsync();
        Task<IEnumerable<Genre>> GetActiveAsync();
        Task<IEnumerable<Genre>> GetByMediaTypeAsync(MediaType mediaType);
        Task<bool> UpdateAsync(Genre genre);
        Task<bool> DeleteAsync(int id);
        
        Task<Genre?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Genre>> SearchAsync(string searchTerm);
    }
}
