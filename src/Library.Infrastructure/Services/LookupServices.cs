using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Library.Core.Interfaces;
using Library.Core.Models;
using Serilog;

namespace Library.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly ILogger _logger;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
            _logger = Log.ForContext<CategoryService>();
        }

        public async Task<Category> CreateAsync(Category category, int createdByUserId)
        {
            _logger.Information("Creating new category: {Name}", category.Name);

            if (await _repository.ExistsByNameAsync(category.Name))
            {
                throw new InvalidOperationException($"Category '{category.Name}' already exists");
            }

            category.CreatedByUserId = createdByUserId;
            category.CreatedAt = DateTime.UtcNow;

            try
            {
                var result = await _repository.CreateAsync(category);
                _logger.Information("Category created successfully with ID {Id}", result.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating category: {Name}", category.Name);
                throw;
            }
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<Category>> GetActiveAsync()
        {
            return await _repository.GetActiveAsync();
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            _logger.Information("Updating category ID {Id}: {Name}", category.Id, category.Name);

            var existingCategory = await _repository.GetByIdAsync(category.Id);
            if (existingCategory == null)
            {
                _logger.Warning("Category with ID {Id} not found for update", category.Id);
                throw new InvalidOperationException("Category not found");
            }

            // Check for name uniqueness (excluding current category)
            if (!await IsNameUniqueAsync(category.Name, category.Id))
            {
                throw new InvalidOperationException($"Category name '{category.Name}' already exists");
            }

            try
            {
                var result = await _repository.UpdateAsync(category);
                _logger.Information("Category ID {Id} updated successfully", category.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating category ID {Id}", category.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.Information("Deactivating category ID {Id}", id);

            var result = await _repository.DeleteAsync(id);
            
            if (result)
                _logger.Information("Category ID {Id} deactivated successfully", id);
            else
                _logger.Warning("Failed to deactivate category ID {Id}", id);

            return result;
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _repository.GetByNameAsync(name);
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            var existingCategory = await _repository.GetByNameAsync(name);
            return existingCategory == null || (excludeId.HasValue && existingCategory.Id == excludeId);
        }

        public async Task<IEnumerable<Category>> SearchAsync(string searchTerm)
        {
            return await _repository.SearchAsync(searchTerm);
        }
    }

    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _repository;
        private readonly ILogger _logger;

        public GenreService(IGenreRepository repository)
        {
            _repository = repository;
            _logger = Log.ForContext<GenreService>();
        }

        public async Task<Genre> CreateAsync(Genre genre, int createdByUserId)
        {
            _logger.Information("Creating new genre: {Name}", genre.Name);

            if (await _repository.ExistsByNameAsync(genre.Name))
            {
                throw new InvalidOperationException($"Genre '{genre.Name}' already exists");
            }

            genre.CreatedByUserId = createdByUserId;
            genre.CreatedAt = DateTime.UtcNow;

            try
            {
                var result = await _repository.CreateAsync(genre);
                _logger.Information("Genre created successfully with ID {Id}", result.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating genre: {Name}", genre.Name);
                throw;
            }
        }

        public async Task<Genre?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<Genre>> GetActiveAsync()
        {
            return await _repository.GetActiveAsync();
        }

        public async Task<IEnumerable<Genre>> GetByMediaTypeAsync(MediaType mediaType)
        {
            return await _repository.GetByMediaTypeAsync(mediaType);
        }

        public async Task<bool> UpdateAsync(Genre genre)
        {
            _logger.Information("Updating genre ID {Id}: {Name}", genre.Id, genre.Name);

            var existingGenre = await _repository.GetByIdAsync(genre.Id);
            if (existingGenre == null)
            {
                _logger.Warning("Genre with ID {Id} not found for update", genre.Id);
                throw new InvalidOperationException("Genre not found");
            }

            // Check for name uniqueness (excluding current genre)
            if (!await IsNameUniqueAsync(genre.Name, genre.Id))
            {
                throw new InvalidOperationException($"Genre name '{genre.Name}' already exists");
            }

            try
            {
                var result = await _repository.UpdateAsync(genre);
                _logger.Information("Genre ID {Id} updated successfully", genre.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating genre ID {Id}", genre.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.Information("Deactivating genre ID {Id}", id);

            var result = await _repository.DeleteAsync(id);
            
            if (result)
                _logger.Information("Genre ID {Id} deactivated successfully", id);
            else
                _logger.Warning("Failed to deactivate genre ID {Id}", id);

            return result;
        }

        public async Task<Genre?> GetByNameAsync(string name)
        {
            return await _repository.GetByNameAsync(name);
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            var existingGenre = await _repository.GetByNameAsync(name);
            return existingGenre == null || (excludeId.HasValue && existingGenre.Id == excludeId);
        }

        public async Task<IEnumerable<Genre>> SearchAsync(string searchTerm)
        {
            return await _repository.SearchAsync(searchTerm);
        }
    }
}
