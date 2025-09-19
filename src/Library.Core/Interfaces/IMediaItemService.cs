using System.Collections.Generic;
using System.Threading.Tasks;
using Library.Core.Models;

namespace Library.Core.Interfaces
{
    public interface IMediaItemService
    {
        // Basic operations
        Task<MediaItem> CreateAsync(MediaItem mediaItem, int createdByUserId);
        Task<MediaItem?> GetByIdAsync(int id);
        Task<IEnumerable<MediaItem>> GetAllAsync();
        Task<bool> UpdateAsync(MediaItem mediaItem, int updatedByUserId);
        Task<bool> DeleteAsync(int id, int deletedByUserId);
        Task<bool> RestoreAsync(int id, int restoredByUserId);
        
        // Search and filtering
        Task<IEnumerable<MediaItem>> SearchAsync(string searchTerm);
        Task<IEnumerable<MediaItem>> GetByTypeAsync(MediaType mediaType);
        Task<IEnumerable<MediaItem>> GetByCategoryAsync(string category);
        Task<IEnumerable<MediaItem>> GetByGenreAsync(string genre);
        Task<IEnumerable<MediaItem>> GetByAuthorAsync(string author);
        Task<MediaItem?> GetByISBNAsync(string isbn);
        
        // Advanced operations
        Task<IEnumerable<MediaItem>> GetAvailableItemsAsync();
        Task<(IEnumerable<MediaItem> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            string? searchTerm = null,
            MediaType? mediaType = null,
            string? category = null,
            string? genre = null);
            
        // Quantity management
        Task<bool> UpdateQuantityAsync(int mediaItemId, int newQuantity, int updatedByUserId);
        Task<bool> AdjustQuantityAsync(int mediaItemId, int adjustment, int updatedByUserId);
        
        // Availability checks
        Task<bool> IsAvailableAsync(int mediaItemId, int requestedQuantity = 1);
        Task<int> GetAvailableQuantityAsync(int mediaItemId);
        
        // Statistics
        Task<int> GetTotalCountAsync();
        Task<int> GetAvailableCountAsync();
        Task<Dictionary<MediaType, int>> GetCountByTypeAsync();
        
        // Validation
        Task<bool> ValidateAsync(MediaItem mediaItem);
        Task<bool> IsUniqueAsync(string title, string? author, string? isbn = null, int? excludeId = null);
        
        // Bulk operations
        Task<IEnumerable<MediaItem>> CreateBulkAsync(IEnumerable<MediaItem> mediaItems, int createdByUserId);
        Task<bool> UpdateQuantitiesBulkAsync(Dictionary<int, int> itemQuantities, int updatedByUserId);
    }
}
