using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Library.Core.Interfaces;
using Library.Core.Models;
using Library.Infrastructure.Data;

namespace Library.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly LibraryDbContext _context;

        public CategoryRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.CreatedByUser)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.CreatedByUser)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetActiveAsync()
        {
            return await _context.Categories
                .Include(c => c.CreatedByUser)
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            category.IsActive = false;
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories
                .Include(c => c.CreatedByUser)
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Categories.AnyAsync(c => c.Name == name);
        }

        public async Task<IEnumerable<Category>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetActiveAsync();

            var lowerSearchTerm = searchTerm.ToLower();

            return await _context.Categories
                .Include(c => c.CreatedByUser)
                .Where(c => c.IsActive && (
                    c.Name.ToLower().Contains(lowerSearchTerm) ||
                    (c.Description != null && c.Description.ToLower().Contains(lowerSearchTerm))
                ))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }

    public class GenreRepository : IGenreRepository
    {
        private readonly LibraryDbContext _context;

        public GenreRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<Genre> CreateAsync(Genre genre)
        {
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
            return genre;
        }

        public async Task<Genre?> GetByIdAsync(int id)
        {
            return await _context.Genres
                .Include(g => g.CreatedByUser)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _context.Genres
                .Include(g => g.CreatedByUser)
                .OrderBy(g => g.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Genre>> GetActiveAsync()
        {
            return await _context.Genres
                .Include(g => g.CreatedByUser)
                .Where(g => g.IsActive)
                .OrderBy(g => g.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Genre>> GetByMediaTypeAsync(MediaType mediaType)
        {
            return await _context.Genres
                .Include(g => g.CreatedByUser)
                .Where(g => g.IsActive && (g.ApplicableToMediaType == null || g.ApplicableToMediaType == mediaType))
                .OrderBy(g => g.Name)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Genre genre)
        {
            _context.Genres.Update(genre);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return false;

            genre.IsActive = false;
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<Genre?> GetByNameAsync(string name)
        {
            return await _context.Genres
                .Include(g => g.CreatedByUser)
                .FirstOrDefaultAsync(g => g.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Genres.AnyAsync(g => g.Name == name);
        }

        public async Task<IEnumerable<Genre>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetActiveAsync();

            var lowerSearchTerm = searchTerm.ToLower();

            return await _context.Genres
                .Include(g => g.CreatedByUser)
                .Where(g => g.IsActive && (
                    g.Name.ToLower().Contains(lowerSearchTerm) ||
                    (g.Description != null && g.Description.ToLower().Contains(lowerSearchTerm))
                ))
                .OrderBy(g => g.Name)
                .ToListAsync();
        }
    }
}
