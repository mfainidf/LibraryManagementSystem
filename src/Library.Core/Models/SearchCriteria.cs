namespace Library.Core.Models
{
    public enum SortCriteria
    {
        Title,
        Author,
        Year,
        Id
    }

    public class SearchCriteria
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public string? Type { get; set; }
        public int? Year { get; set; }
        public SortCriteria SortBy { get; set; } = SortCriteria.Title;
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 20;
    }
}
