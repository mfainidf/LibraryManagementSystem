using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Library.Core.Interfaces;
using Library.Core.Models;
using Library.Infrastructure.Data;

namespace Library.Infrastructure.Repositories
{
    public class MediaItemRepository : IMediaItemRepository
    {
        private readonly LibraryDbContext _context;

        public MediaItemRepository(LibraryDbContext context)
        {
            _context = context;
        }

        // Basic CRUD operations
        public async Task<MediaItem> CreateAsync(MediaItem mediaItem)
        {
            _context.MediaItems.Add(mediaItem);
            await _context.SaveChangesAsync();
            return mediaItem;
        }

        public async Task<MediaItem?> GetByIdAsync(int id)
        {
            return await _context.MediaItems
                .Include(m => m.CreatedByUser)
                .Include(m => m.UpdatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        }

        public async Task<MediaItem?> GetByIdIncludingDeletedAsync(int id)
        {
            return await _context.MediaItems
                .Include(m => m.CreatedByUser)
                .Include(m => m.UpdatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<MediaItem>> GetAllAsync()
        {
            return await _context.MediaItems
                .Include(m => m.CreatedByUser)
                .Include(m => m.UpdatedByUser)
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<MediaItem>> GetAllIncludingDeletedAsync()
        {
            return await _context.MediaItems
                .Include(m => m.CreatedByUser)
                .Include(m => m.UpdatedByUser)
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(MediaItem mediaItem)
        {
            _context.MediaItems.Update(mediaItem);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var mediaItem = await _context.MediaItems.FindAsync(id);
            if (mediaItem == null) return false;

            mediaItem.MarkAsDeleted();
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var mediaItem = await _context.MediaItems.FindAsync(id);
            if (mediaItem == null) return false;

            _context.MediaItems.Remove(mediaItem);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        // Search and filtering
        public async Task<IEnumerable<MediaItem>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            var lowerSearchTerm = searchTerm.ToLower();

            return await _context.MediaItems
                .Include(m => m.CreatedByUser)
                .Include(m => m.UpdatedByUser)
                .Where(m => !m.IsDeleted && (
                    m.Title.ToLower().Contains(lowerSearchTerm) ||
                    (m.Author != null && m.Author.ToLower().Contains(lowerSearchTerm)) ||
                    (m.ISBN != null && m.ISBN.ToLower().Contains(lowerSearchTerm)) ||
                    (m.Description != null && m.Description.ToLower().Contains(lowerSearchTerm))
                ))
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<MediaItem>> GetByTypeAsync(MediaType mediaType)
        {
            return await _context.MediaItems
                .Include(m => m.CreatedByUser)
                .Include(m => m.UpdatedByUser)
                .Where(m => !m.IsDeleted && m.Type == mediaType)
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<MediaItem>> GetByCategoryAsync(string category)
        {
            return await _context.MediaItems
                .Include(m => m.CreatedByUser)
                .Include(m => m.UpdatedByUser)
                .Where(m => !m.IsDeleted && m.Category == category)
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<MediaItem>> GetByGenreAsync(string genre)
        {
            return await _context.MediaItems
                .Include(m => m.CreatedByUser)
                .Include(m => m.UpdatedByUser)
                .Where(m => !m.IsDeleted && m.Genre == genre)
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<MediaItem>> GetByAuthorAsync(string author)
        {
            return await _context.MediaItems
                .Include(m => m.CreatedByUser)
                .Include(m => m.UpdatedByUser)
                .Where(m => !m.IsDeleted && m.Author == author)
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<MediaItem>> GetByISBNAsync(string isbn)
        {
            return await _context.MediaItems
                .Include(m => m.CreatedByUser)
                .Include(m => m.UpdatedByUser)
                .Where(m => !m.IsDeleted && m.ISBN == isbn)
                .ToListAsync();
        }

        // Advanced filtering
        public async Task<IEnumerable<MediaItem>> GetAvailableAsync()
        {
            return await _context.MediaItems
                .Include(m => m.CreatedByUser)
                .Include(m => m.UpdatedByUser)
                .Where(m => !m.IsDeleted && m.AvailableQuantity > 0)
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<MediaItem>> GetUnavailableAsync()
        {
            return await _context.MediaItems
                .Include(m => m.CreatedByUser)
                .Include(m => m.UpdatedByUser)
                .Where(m => !m.IsDeleted && m.AvailableQuantity <= 0)
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<MediaItem>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.MediaItems
                .Include(m => m.CreatedByUser)
                .Include(m => m.UpdatedByUser)
                .Where(m => !m.IsDeleted && 
                           m.PublicationDate.HasValue && 
                           m.PublicationDate >= startDate && 
                           m.PublicationDate <= endDate)
                .OrderBy(m => m.PublicationDate)
                .ToListAsync();
        }

        // Pagination
        public async Task<(IEnumerable<MediaItem> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            string? searchTerm = null,
            MediaType? mediaType = null,
            string? category = null,
            string? genre = null,
            bool includeDeleted = false)
        {
            var query = _context.MediaItems
                .Include(m => m.CreatedByUser)
                .Include(m => m.UpdatedByUser)
                .AsQueryable();

            // Apply filters
            if (!includeDeleted)
                query = query.Where(m => !m.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(m => 
                    m.Title.ToLower().Contains(lowerSearchTerm) ||
                    (m.Author != null && m.Author.ToLower().Contains(lowerSearchTerm)) ||
                    (m.ISBN != null && m.ISBN.ToLower().Contains(lowerSearchTerm))
                );
            }

            if (mediaType.HasValue)
                query = query.Where(m => m.Type == mediaType);

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(m => m.Category == category);

            if (!string.IsNullOrWhiteSpace(genre))
                query = query.Where(m => m.Genre == genre);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(m => m.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // Statistics and counts
        public async Task<int> GetTotalCountAsync()
        {
            return await _context.MediaItems.CountAsync(m => !m.IsDeleted);
        }

        public async Task<int> GetAvailableCountAsync()
        {
            return await _context.MediaItems.CountAsync(m => !m.IsDeleted && m.AvailableQuantity > 0);
        }

        public async Task<int> GetCountByTypeAsync(MediaType mediaType)
        {
            return await _context.MediaItems.CountAsync(m => !m.IsDeleted && m.Type == mediaType);
        }

        // Existence checks
        public async Task<bool> ExistsByISBNAsync(string isbn)
        {
            return await _context.MediaItems.AnyAsync(m => !m.IsDeleted && m.ISBN == isbn);
        }

        public async Task<bool> ExistsByTitleAndAuthorAsync(string title, string? author)
        {
            return await _context.MediaItems.AnyAsync(m => 
                !m.IsDeleted && 
                m.Title == title && 
                m.Author == author);
        }

        // Bulk operations
        public async Task<bool> UpdateQuantitiesAsync(Dictionary<int, int> itemQuantities)
        {
            foreach (var item in itemQuantities)
            {
                var mediaItem = await _context.MediaItems.FindAsync(item.Key);
                if (mediaItem != null && !mediaItem.IsDeleted)
                {
                    mediaItem.UpdateQuantity(item.Value);
                    mediaItem.UpdatedAt = DateTime.UtcNow;
                }
            }

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IEnumerable<MediaItem>> CreateBulkAsync(IEnumerable<MediaItem> mediaItems)
        {
            var itemsList = mediaItems.ToList();
            _context.MediaItems.AddRange(itemsList);
            await _context.SaveChangesAsync();
            return itemsList;
        }
    }
}
