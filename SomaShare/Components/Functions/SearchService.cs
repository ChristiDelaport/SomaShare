using Microsoft.EntityFrameworkCore;
using SomaShare.Components.Model;

namespace SomaShare.Services
{

    public interface ISearchService
    {

        Task<PagedResult<ListingAd>> SearchListingsAsync(ListingSearchParams searchParams);

        Task<PagedResult<WantedAd>> SearchWantedAdsAsync(WantedAdSearchParams searchParams);

        Task<PagedResult<ListingAd>> GetUserListingsAsync(string userId, int pageNumber = 1, int pageSize = 10);

        Task<PagedResult<Offer>> GetReceivedOffersAsync(string sellerId, int pageNumber = 1, int pageSize = 10);

        Task<PagedResult<Offer>> GetMadeOffersAsync(string buyerId, int pageNumber = 1, int pageSize = 10);

        Task<PagedResult<Transaction>> GetUserTransactionsAsync(string userId, bool asBuyer = true, int pageNumber = 1, int pageSize = 10);

        Task<PagedResult<Review>> GetUserReviewsAsync(string userId, int pageNumber = 1, int pageSize = 10);

        Task<PagedResult<Review>> GetUserReviewsGivenAsync(string userId, int pageNumber = 1, int pageSize = 10);

        Task<DashboardSummary> GetDashboardSummaryAsync(string userId);
    }

    public class SearchService : ISearchService
    {
        private readonly SomaContext _context;

        public SearchService(SomaContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ListingAd>> SearchListingsAsync(ListingSearchParams searchParams)
        {

            var query = _context.ListingAds
                .AsNoTracking()
                .Include(l => l.Textbook)
                .Include(l => l.User)
                .Include(l => l.Genre)
                .AsQueryable();

            // Apply filters 
            if (searchParams.IsActive)
            {
                query = query.Where(l => l.IsActive);
            }

            // Search by title
            if (!string.IsNullOrWhiteSpace(searchParams.SearchTitle))
            {
                var searchTerm = searchParams.SearchTitle.ToLower();
                query = query.Where(l => l.Textbook.Title.ToLower().Contains(searchTerm));
            }

            // Search by author
            if (!string.IsNullOrWhiteSpace(searchParams.SearchAuthor))
            {
                var searchTerm = searchParams.SearchAuthor.ToLower();
                query = query.Where(l => l.Textbook.Author.ToLower().Contains(searchTerm));
            }

            // Search by ISBN
            if (!string.IsNullOrWhiteSpace(searchParams.SearchISBN))
            {
                var searchTerm = searchParams.SearchISBN.ToLower();
                query = query.Where(l => l.Textbook.ISBN.ToLower().Contains(searchTerm));
            }

            // Search by course code 
            if (!string.IsNullOrWhiteSpace(searchParams.SearchCourseCode))
            {
                var searchTerm = searchParams.SearchCourseCode.ToLower();
                query = query.Where(l => l.Description.ToLower().Contains(searchTerm) ||
                                         l.Textbook.Description.ToLower().Contains(searchTerm));
            }

            // Filter by price range
            if (searchParams.MinPrice.HasValue)
            {
                query = query.Where(l => l.Price >= searchParams.MinPrice.Value);
            }

            if (searchParams.MaxPrice.HasValue)
            {
                query = query.Where(l => l.Price <= searchParams.MaxPrice.Value);
            }

            // Filter by condition
            if (!string.IsNullOrWhiteSpace(searchParams.Condition))
            {
                query = query.Where(l => l.Condition == searchParams.Condition);
            }

            // Filter by campus location
            if (!string.IsNullOrWhiteSpace(searchParams.CampusLocation))
            {
                var searchTerm = searchParams.CampusLocation.ToLower();
                query = query.Where(l => l.CampusLocation.ToLower().Contains(searchTerm));
            }

            // Filter by genre
            if (searchParams.GenreId.HasValue)
            {
                query = query.Where(l => l.Genre_ID == searchParams.GenreId.Value);
            }

            // Apply sorting
            query = ApplySorting(query, searchParams.SortBy);

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var skip = (searchParams.PageNumber - 1) * searchParams.PageSize;
            var items = await query
                .Skip(skip)
                .Take(searchParams.PageSize)
                .ToListAsync();

            return new PagedResult<ListingAd>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = searchParams.PageNumber,
                PageSize = searchParams.PageSize
            };
        }

        public async Task<PagedResult<WantedAd>> SearchWantedAdsAsync(WantedAdSearchParams searchParams)
        {
            var query = _context.WantedAds
                .AsNoTracking()
                .Include(w => w.Textbook)
                .Include(w => w.User)
                .Include(w => w.Genre)
                .AsQueryable();

            if (searchParams.IsActive)
            {
                query = query.Where(w => w.IsActive);
            }

            // Search by title
            if (!string.IsNullOrWhiteSpace(searchParams.SearchTitle))
            {
                var searchTerm = searchParams.SearchTitle.ToLower();
                query = query.Where(w => w.Textbook.Title.ToLower().Contains(searchTerm));
            }

            // Search by author
            if (!string.IsNullOrWhiteSpace(searchParams.SearchAuthor))
            {
                var searchTerm = searchParams.SearchAuthor.ToLower();
                query = query.Where(w => w.Textbook.Author.ToLower().Contains(searchTerm));
            }

            // Filter by max price
            if (searchParams.MaxPrice.HasValue)
            {
                query = query.Where(w => !w.MaxPrice.HasValue || w.MaxPrice.Value >= searchParams.MaxPrice.Value);
            }

            // Filter by genre
            if (searchParams.GenreId.HasValue)
            {
                query = query.Where(w => w.Genre_ID == searchParams.GenreId.Value);
            }

            // Apply sorting
            query = ApplySortingWantedAds(query, searchParams.SortBy);

            var totalCount = await query.CountAsync();
            var skip = (searchParams.PageNumber - 1) * searchParams.PageSize;
            var items = await query
                .Skip(skip)
                .Take(searchParams.PageSize)
                .ToListAsync();

            return new PagedResult<WantedAd>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = searchParams.PageNumber,
                PageSize = searchParams.PageSize
            };
        }


        public async Task<PagedResult<ListingAd>> GetUserListingsAsync(string userId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.ListingAds
                .AsNoTracking()
                .Where(l => l.User_ID == userId)
                .Include(l => l.Textbook)
                .Include(l => l.Genre)
                .Include(l => l.Offers)
                .OrderByDescending(l => l.Date_Posted);

            var totalCount = await query.CountAsync();
            var skip = (pageNumber - 1) * pageSize;
            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ListingAd>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public async Task<PagedResult<Offer>> GetReceivedOffersAsync(string sellerId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Offers
                .AsNoTracking()
                .Where(o => o.Seller_ID == sellerId)
                .Include(o => o.Buyer)
                .Include(o => o.ListingAd)
                    .ThenInclude(l => l.Textbook)
                .OrderByDescending(o => o.DateCreated);

            var totalCount = await query.CountAsync();
            var skip = (pageNumber - 1) * pageSize;
            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Offer>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<Offer>> GetMadeOffersAsync(string buyerId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Offers
                .AsNoTracking()
                .Where(o => o.Buyer_ID == buyerId)
                .Include(o => o.Seller)
                .Include(o => o.ListingAd)
                    .ThenInclude(l => l.Textbook)
                .OrderByDescending(o => o.DateCreated);

            var totalCount = await query.CountAsync();
            var skip = (pageNumber - 1) * pageSize;
            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Offer>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<Transaction>> GetUserTransactionsAsync(string userId, bool asBuyer = true, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Transactions
                .AsNoTracking()
                .Include(t => t.Offer)
                    .ThenInclude(o => o.ListingAd)
                        .ThenInclude(l => l.Textbook)
                .Include(t => t.Buyer)
                .Include(t => t.Seller)
                .Include(t => t.Reviews)
                .AsQueryable();

            if (asBuyer)
            {
                query = query.Where(t => t.Buyer_Id == userId);
            }
            else
            {
                query = query.Where(t => t.Seller_Id == userId);
            }

            query = query.OrderByDescending(t => t.Date_Created);

            var totalCount = await query.CountAsync();
            var skip = (pageNumber - 1) * pageSize;
            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Transaction>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<Review>> GetUserReviewsAsync(string userId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Reviews
                .AsNoTracking()
                .Where(r => r.Reviewee_ID == userId)
                .Include(r => r.Reviewer)
                .Include(r => r.Transaction)
                .OrderByDescending(r => r.Date_Created);

            var totalCount = await query.CountAsync();
            var skip = (pageNumber - 1) * pageSize;
            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Review>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public async Task<PagedResult<Review>> GetUserReviewsGivenAsync(string userId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Reviews
                .AsNoTracking()
                .Where(r => r.Reviewer_ID == userId)
                .Include(r => r.Reviewee)
                .Include(r => r.Transaction)
                .OrderByDescending(r => r.Date_Created);

            var totalCount = await query.CountAsync();
            var skip = (pageNumber - 1) * pageSize;
            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Review>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<DashboardSummary> GetDashboardSummaryAsync(string userId)
        {
            var myListingsCount = await _context.ListingAds
                .CountAsync(l => l.User_ID == userId && l.IsActive);

            var receivedOffersCount = await _context.Offers
                .CountAsync(o => o.Seller_ID == userId && o.Status == "Pending");

            var madeOffersCount = await _context.Offers
                .CountAsync(o => o.Buyer_ID == userId && o.Status == "Pending");

            var transactionsCount = await _context.Transactions
                .CountAsync(t => (t.Buyer_Id == userId || t.Seller_Id == userId) && t.Status == "Pending");

            var completedTransactionsCount = await _context.Transactions
                .CountAsync(t => (t.Buyer_Id == userId || t.Seller_Id == userId) && t.Status == "Completed");

            var reviewsReceivedCount = await _context.Reviews
                .CountAsync(r => r.Reviewee_ID == userId);

            var averageRating = await _context.Reviews
                .Where(r => r.Reviewee_ID == userId)
                .AverageAsync(r => (double?)r.Rating) ?? 0;

            return new DashboardSummary
            {
                MyListingsCount = myListingsCount,
                ReceivedOffersCount = receivedOffersCount,
                MadeOffersCount = madeOffersCount,
                PendingTransactionsCount = transactionsCount,
                CompletedTransactionsCount = completedTransactionsCount,
                ReviewsReceivedCount = reviewsReceivedCount,
                AverageRating = averageRating
            };
        }


        private static IQueryable<ListingAd> ApplySorting(IQueryable<ListingAd> query, string sortBy)
        {
            return sortBy switch
            {
                "price_asc" => query.OrderBy(l => l.Price),
                "price_desc" => query.OrderByDescending(l => l.Price),
                "popular" => query.OrderByDescending(l => l.Offers.Count()),
                _ => query.OrderByDescending(l => l.Date_Posted) // newest
            };
        }

        private static IQueryable<WantedAd> ApplySortingWantedAds(IQueryable<WantedAd> query, string sortBy)
        {
            return sortBy switch
            {
                "price_asc" => query.OrderBy(w => w.MaxPrice),
                "price_desc" => query.OrderByDescending(w => w.MaxPrice),
                _ => query.OrderByDescending(w => w.Date_Posted)
            };
        }
    }

    public class DashboardSummary
    {
        public int MyListingsCount { get; set; }
        public int ReceivedOffersCount { get; set; }
        public int MadeOffersCount { get; set; }
        public int PendingTransactionsCount { get; set; }
        public int CompletedTransactionsCount { get; set; }
        public int ReviewsReceivedCount { get; set; }
        public double AverageRating { get; set; }
    }
}
