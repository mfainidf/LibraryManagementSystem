using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Core.Interfaces;
using Library.Core.Models;
using Serilog;

namespace Library.Infrastructure.Services
{
    public class MediaItemService : IMediaItemService
    {
        private readonly IMediaItemRepository _repository;
        private readonly ILogger _logger;

        public MediaItemService(IMediaItemRepository repository)
        {
            _repository = repository;
            _logger = Log.ForContext<MediaItemService>();
        }

        // Basic operations
        public async Task<MediaItem> CreateAsync(MediaItem mediaItem, int createdByUserId)
        {
            _logger.Information("Creating new media item: {Title} by {Author}", mediaItem.Title, mediaItem.Author);

            // Validate the media item
            if (!await ValidateAsync(mediaItem))
            {
                throw new ArgumentException("Invalid media item data");
            }

            // Check for duplicates
            if (!string.IsNullOrEmpty(mediaItem.ISBN) && await _repository.ExistsByISBNAsync(mediaItem.ISBN))
            {
                throw new InvalidOperationException($"Media item with ISBN {mediaItem.ISBN} already exists");
            }

            if (await _repository.ExistsByTitleAndAuthorAsync(mediaItem.Title, mediaItem.Author))
            {
                throw new InvalidOperationException($"Media item '{mediaItem.Title}' by '{mediaItem.Author}' already exists");
            }

            // Set metadata
            mediaItem.CreatedByUserId = createdByUserId;
            mediaItem.CreatedAt = DateTime.UtcNow;
            mediaItem.AvailableQuantity = mediaItem.Quantity; // Initially all items are available

            try
            {
                var result = await _repository.CreateAsync(mediaItem);
                _logger.Information("Media item created successfully with ID {Id}", result.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating media item: {Title}", mediaItem.Title);
                throw;
            }
        }

        public async Task<MediaItem?> GetByIdAsync(int id)
        {
            _logger.Debug("Retrieving media item with ID {Id}", id);
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<MediaItem>> GetAllAsync()
        {
            _logger.Debug("Retrieving all media items");
            return await _repository.GetAllAsync();
        }

        public async Task<bool> UpdateAsync(MediaItem mediaItem, int updatedByUserId)
        {
            _logger.Information("Updating media item ID {Id}: {Title}", mediaItem.Id, mediaItem.Title);

            var existingItem = await _repository.GetByIdAsync(mediaItem.Id);
            if (existingItem == null)
            {
                _logger.Warning("Media item with ID {Id} not found for update", mediaItem.Id);
                throw new InvalidOperationException("Media item not found");
            }

            // Validate the updated data
            if (!await ValidateAsync(mediaItem))
            {
                throw new ArgumentException("Invalid media item data");
            }

            // Check for ISBN duplicates (excluding current item)
            if (!string.IsNullOrEmpty(mediaItem.ISBN))
            {
                var duplicateItems = await _repository.GetByISBNAsync(mediaItem.ISBN);
                if (duplicateItems.Any(d => d.Id != mediaItem.Id))
                {
                    throw new InvalidOperationException($"Another media item with ISBN {mediaItem.ISBN} already exists");
                }
            }

            // Set update metadata
            mediaItem.UpdatedByUserId = updatedByUserId;
            mediaItem.UpdatedAt = DateTime.UtcNow;

            try
            {
                var result = await _repository.UpdateAsync(mediaItem);
                _logger.Information("Media item ID {Id} updated successfully", mediaItem.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating media item ID {Id}", mediaItem.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id, int deletedByUserId)
        {
            _logger.Information("Soft deleting media item ID {Id}", id);

            var mediaItem = await _repository.GetByIdAsync(id);
            if (mediaItem == null)
            {
                _logger.Warning("Media item with ID {Id} not found for deletion", id);
                return false;
            }

            mediaItem.UpdatedByUserId = deletedByUserId;
            var result = await _repository.DeleteAsync(id);
            
            if (result)
                _logger.Information("Media item ID {Id} soft deleted successfully", id);
            else
                _logger.Warning("Failed to soft delete media item ID {Id}", id);

            return result;
        }

        public async Task<bool> RestoreAsync(int id, int restoredByUserId)
        {
            _logger.Information("Restoring media item ID {Id}", id);

            var mediaItem = await _repository.GetByIdIncludingDeletedAsync(id);
            if (mediaItem == null)
            {
                _logger.Warning("Media item with ID {Id} not found for restoration", id);
                return false;
            }

            if (!mediaItem.IsDeleted)
            {
                _logger.Information("Media item ID {Id} is not deleted, no restoration needed", id);
                return true;
            }

            mediaItem.Restore();
            mediaItem.UpdatedByUserId = restoredByUserId;

            var result = await _repository.UpdateAsync(mediaItem);
            
            if (result)
                _logger.Information("Media item ID {Id} restored successfully", id);
            else
                _logger.Warning("Failed to restore media item ID {Id}", id);

            return result;
        }

        // Search and filtering
        public async Task<IEnumerable<MediaItem>> SearchAsync(string searchTerm)
        {
            _logger.Debug("Searching media items with term: {SearchTerm}", searchTerm);
            return await _repository.SearchAsync(searchTerm);
        }

        public async Task<IEnumerable<MediaItem>> GetByTypeAsync(MediaType mediaType)
        {
            _logger.Debug("Retrieving media items of type: {MediaType}", mediaType);
            return await _repository.GetByTypeAsync(mediaType);
        }

        public async Task<IEnumerable<MediaItem>> GetByCategoryAsync(string category)
        {
            _logger.Debug("Retrieving media items in category: {Category}", category);
            return await _repository.GetByCategoryAsync(category);
        }

        public async Task<IEnumerable<MediaItem>> GetByGenreAsync(string genre)
        {
            _logger.Debug("Retrieving media items in genre: {Genre}", genre);
            return await _repository.GetByGenreAsync(genre);
        }

        public async Task<IEnumerable<MediaItem>> GetByAuthorAsync(string author)
        {
            _logger.Debug("Retrieving media items by author: {Author}", author);
            return await _repository.GetByAuthorAsync(author);
        }

        public async Task<MediaItem?> GetByISBNAsync(string isbn)
        {
            _logger.Debug("Retrieving media item by ISBN: {ISBN}", isbn);
            var items = await _repository.GetByISBNAsync(isbn);
            return items.FirstOrDefault();
        }

        // Advanced operations
        public async Task<IEnumerable<MediaItem>> GetAvailableItemsAsync()
        {
            _logger.Debug("Retrieving available media items");
            return await _repository.GetAvailableAsync();
        }

        public async Task<(IEnumerable<MediaItem> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            string? searchTerm = null,
            MediaType? mediaType = null,
            string? category = null,
            string? genre = null)
        {
            _logger.Debug("Retrieving paged media items: Page {Page}, Size {Size}", pageNumber, pageSize);
            return await _repository.GetPagedAsync(pageNumber, pageSize, searchTerm, mediaType, category, genre);
        }

        // Quantity management
        public async Task<bool> UpdateQuantityAsync(int mediaItemId, int newQuantity, int updatedByUserId)
        {
            _logger.Information("Updating quantity for media item ID {Id} to {Quantity}", mediaItemId, newQuantity);

            var mediaItem = await _repository.GetByIdAsync(mediaItemId);
            if (mediaItem == null)
            {
                _logger.Warning("Media item with ID {Id} not found for quantity update", mediaItemId);
                throw new InvalidOperationException("Media item not found");
            }

            var oldQuantity = mediaItem.Quantity;
            mediaItem.UpdateQuantity(newQuantity);
            mediaItem.UpdatedByUserId = updatedByUserId;
            mediaItem.UpdatedAt = DateTime.UtcNow;

            var result = await _repository.UpdateAsync(mediaItem);
            
            if (result)
                _logger.Information("Quantity updated for media item ID {Id}: {OldQuantity} → {NewQuantity}", 
                    mediaItemId, oldQuantity, newQuantity);

            return result;
        }

        public async Task<bool> AdjustQuantityAsync(int mediaItemId, int adjustment, int updatedByUserId)
        {
            _logger.Information("Adjusting quantity for media item ID {Id} by {Adjustment}", mediaItemId, adjustment);

            var mediaItem = await _repository.GetByIdAsync(mediaItemId);
            if (mediaItem == null)
            {
                _logger.Warning("Media item with ID {Id} not found for quantity adjustment", mediaItemId);
                throw new InvalidOperationException("Media item not found");
            }

            var newQuantity = mediaItem.Quantity + adjustment;
            return await UpdateQuantityAsync(mediaItemId, newQuantity, updatedByUserId);
        }

        // Availability checks
        public async Task<bool> IsAvailableAsync(int mediaItemId, int requestedQuantity = 1)
        {
            var mediaItem = await _repository.GetByIdAsync(mediaItemId);
            return mediaItem?.CanBorrow(requestedQuantity) ?? false;
        }

        public async Task<int> GetAvailableQuantityAsync(int mediaItemId)
        {
            var mediaItem = await _repository.GetByIdAsync(mediaItemId);
            return mediaItem?.AvailableQuantity ?? 0;
        }

        // Statistics
        public async Task<int> GetTotalCountAsync()
        {
            return await _repository.GetTotalCountAsync();
        }

        public async Task<int> GetAvailableCountAsync()
        {
            return await _repository.GetAvailableCountAsync();
        }

        public async Task<Dictionary<MediaType, int>> GetCountByTypeAsync()
        {
            var result = new Dictionary<MediaType, int>();
            
            foreach (MediaType mediaType in Enum.GetValues<MediaType>())
            {
                var count = await _repository.GetCountByTypeAsync(mediaType);
                result[mediaType] = count;
            }

            return result;
        }

        // Validation
        public async Task<bool> ValidateAsync(MediaItem mediaItem)
        {
            if (string.IsNullOrWhiteSpace(mediaItem.Title))
                return false;

            if (mediaItem.Quantity < 0)
                return false;

            if (mediaItem.AvailableQuantity < 0 || mediaItem.AvailableQuantity > mediaItem.Quantity)
                return false;

            // ISBN validation for books
            if (mediaItem.Type.RequiresISBN() && string.IsNullOrWhiteSpace(mediaItem.ISBN))
                return false;

            return await Task.FromResult(true);
        }

        public async Task<bool> IsUniqueAsync(string title, string? author, string? isbn = null, int? excludeId = null)
        {
            // Check ISBN uniqueness
            if (!string.IsNullOrEmpty(isbn))
            {
                var existingByISBN = await _repository.GetByISBNAsync(isbn);
                if (existingByISBN.Any(item => excludeId == null || item.Id != excludeId))
                    return false;
            }

            // Check title/author combination
            var existingByTitleAuthor = await _repository.ExistsByTitleAndAuthorAsync(title, author);
            if (existingByTitleAuthor && excludeId == null)
                return false;

            return true;
        }

        // Bulk operations
        public async Task<IEnumerable<MediaItem>> CreateBulkAsync(IEnumerable<MediaItem> mediaItems, int createdByUserId)
        {
            _logger.Information("Creating bulk media items: {Count} items", mediaItems.Count());

            var itemsList = mediaItems.ToList();
            
            // Validate all items
            foreach (var item in itemsList)
            {
                if (!await ValidateAsync(item))
                    throw new ArgumentException($"Invalid media item: {item.Title}");

                item.CreatedByUserId = createdByUserId;
                item.CreatedAt = DateTime.UtcNow;
                item.AvailableQuantity = item.Quantity;
            }

            try
            {
                var result = await _repository.CreateBulkAsync(itemsList);
                _logger.Information("Bulk creation completed: {Count} items created", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during bulk creation of media items");
                throw;
            }
        }

        public async Task<bool> UpdateQuantitiesBulkAsync(Dictionary<int, int> itemQuantities, int updatedByUserId)
        {
            _logger.Information("Bulk updating quantities for {Count} items", itemQuantities.Count);

            try
            {
                var result = await _repository.UpdateQuantitiesAsync(itemQuantities);
                
                if (result)
                    _logger.Information("Bulk quantity update completed successfully");
                else
                    _logger.Warning("Bulk quantity update failed");

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during bulk quantity update");
                throw;
            }
        }
    }
}
