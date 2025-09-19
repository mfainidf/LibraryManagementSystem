using Library.Core.Models;
using Library.Core.Interfaces;

namespace Library.Application.Services
{
    public class SearchService
    {
        private readonly IMediaItemRepository _mediaRepository;

        public SearchService(IMediaItemRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public async Task<(IEnumerable<MediaItem> Items, int TotalCount)> SearchAsync(SearchCriteria criteria)
        {
            // Use repository's paging implementation (page number is 1-based)
            var paged = await _mediaRepository.GetPagedAsync(
                criteria.Page + 1,
                criteria.PageSize,
                string.IsNullOrWhiteSpace(criteria.Title) ? null : criteria.Title,
                string.IsNullOrWhiteSpace(criteria.Type) ? null : TryParseMediaType(criteria.Type),
                null, // category not present in SearchCriteria
                string.IsNullOrWhiteSpace(criteria.Genre) ? null : criteria.Genre,
                includeDeleted: false);

            var items = paged.Items;
            var total = paged.TotalCount;

            // Note: Repository may not support server-side sorting; sorting could be applied here if needed
            return (items, total);
        }

        private MediaType? TryParseMediaType(string? type)
        {
            if (string.IsNullOrWhiteSpace(type)) return null;
            if (Enum.TryParse<MediaType>(type, true, out var mt)) return mt;
            return null;
        }
    }
}
