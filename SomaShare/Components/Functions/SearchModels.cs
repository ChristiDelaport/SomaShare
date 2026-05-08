using System.ComponentModel.DataAnnotations;

namespace SomaShare.Services
{

    public class ListingSearchParams
    {
        [StringLength(100)]
        public string? SearchTitle { get; set; }

        [StringLength(100)]
        public string? SearchAuthor { get; set; }

        [StringLength(20)]
        public string? SearchISBN { get; set; }

        [StringLength(50)]
        public string? SearchCourseCode { get; set; }

        [Range(0, 10000)]
        public decimal? MinPrice { get; set; }

        [Range(0, 10000)]
        public decimal? MaxPrice { get; set; }

        [StringLength(50)]
        public string? Condition { get; set; }

        [StringLength(100)]
        public string? CampusLocation { get; set; }

        public int? GenreId { get; set; }

        public string SortBy { get; set; } = "newest";

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Parameters for searching wanted ads
    /// </summary>
    public class WantedAdSearchParams
    {
        [StringLength(100)]
        public string? SearchTitle { get; set; }

        [StringLength(100)]
        public string? SearchAuthor { get; set; }

        [Range(0, 10000)]
        public decimal? MaxPrice { get; set; }

        public int? GenreId { get; set; }

        public string SortBy { get; set; } = "newest";

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public bool IsActive { get; set; } = true;
    }
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;

        public int StartIndex => (PageNumber - 1) * PageSize + 1;

        public int EndIndex => Math.Min(PageNumber * PageSize, TotalCount);
    }
}
