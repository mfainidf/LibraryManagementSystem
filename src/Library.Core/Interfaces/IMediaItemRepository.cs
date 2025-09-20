using System.Collections.Generic;
using System.Threading.Tasks;
using Library.Core.Models;

namespace Library.Core.Interfaces
{
    public interface IMediaItemRepository
    {
        // Basic CRUD operations
        Task<MediaItem> CreateAsync(MediaItem mediaItem);
        Task<MediaItem?> GetByIdAsync(int id);
        Task<MediaItem?> GetByIdIncludingDeletedAsync(int id);
        Task<IEnumerable<MediaItem>> GetAllAsync();
        Task<IEnumerable<MediaItem>> GetAllIncludingDeletedAsync();
        Task<bool> UpdateAsync(MediaItem mediaItem);
        Task<bool> DeleteAsync(int id); // Soft delete
        Task<bool> HardDeleteAsync(int id); // Hard delete
        
        // Search and filtering
        Task<IEnumerable<MediaItem>> SearchAsync(string searchTerm);
        Task<IEnumerable<MediaItem>> GetByTypeAsync(MediaType mediaType);
        Task<IEnumerable<MediaItem>> GetByCategoryAsync(string category);
        Task<IEnumerable<MediaItem>> GetByGenreAsync(string genre);
        Task<IEnumerable<MediaItem>> GetByAuthorAsync(string author);
        Task<IEnumerable<MediaItem>> GetByISBNAsync(string isbn);
        
        // Advanced filtering
        Task<IEnumerable<MediaItem>> GetAvailableAsync();
        Task<IEnumerable<MediaItem>> GetUnavailableAsync();
        Task<IEnumerable<MediaItem>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        // Pagination
        Task<(IEnumerable<MediaItem> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            string? searchTerm = null,
            MediaType? mediaType = null,
            string? category = null,
            string? genre = null,
            bool includeDeleted = false);
            
        // Statistics and counts
        Task<int> GetTotalCountAsync();
        Task<int> GetAvailableCountAsync();
        Task<int> GetCountByTypeAsync(MediaType mediaType);
        
        // Existence checks
        Task<bool> ExistsByISBNAsync(string isbn);
        Task<bool> ExistsByTitleAndAuthorAsync(string title, string? author);
        
        // Bulk operations
        Task<bool> UpdateQuantitiesAsync(Dictionary<int, int> itemQuantities);
        Task<IEnumerable<MediaItem>> CreateBulkAsync(IEnumerable<MediaItem> mediaItems);
    }
}
