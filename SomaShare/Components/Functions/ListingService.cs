using Microsoft.EntityFrameworkCore;
using SomaShare.Components.Model;

namespace SomaShare.Services
{
    public interface IListingService
    {
        Task<List<ListingAd>> GetUserListingsAsync(string userId);
        Task<ListingAd?> GetListingAsync(int listingId);
        Task<ListingAd> CreateListingAsync(Textbook textbook, ListingAd listing);
        Task<ListingAd> UpdateListingAsync(ListingAd listing);
        Task DeleteListingAsync(int listingId);
        Task<List<ListingAd>> GetActiveBrowseableListingsAsync(string excludeUserId);
        Task<List<ListingAd>> GetFilteredListingsAsync(string? searchTitle, string? genreId, string excludeUserId);
    }

    public class ListingService : IListingService
    {
        private readonly SomaContext _context;

        public ListingService(SomaContext context)
        {
            _context = context;
        }

        public async Task<List<ListingAd>> GetUserListingsAsync(string userId)
        {
            return await _context.ListingAds
                .Where(l => l.User_ID == userId)
                .Include(l => l.Textbook)
                .Include(l => l.Offers)
                .OrderByDescending(l => l.Date_Posted)
                .ToListAsync();
        }

        public async Task<ListingAd?> GetListingAsync(int listingId)
        {
            return await _context.ListingAds
                .Include(l => l.Textbook)
                .Include(l => l.User)
                .Include(l => l.Genre)
                .FirstOrDefaultAsync(l => l.ListingAd_ID == listingId);
        }

        public async Task<ListingAd> CreateListingAsync(Textbook textbook, ListingAd listing)
        {
            await _context.Textbooks.AddAsync(textbook);
            await _context.SaveChangesAsync();

            listing.Textbook_ID = textbook.Textbook_ID;
            await _context.ListingAds.AddAsync(listing);
            await _context.SaveChangesAsync();

            return listing;
        }

        public async Task<ListingAd> UpdateListingAsync(ListingAd listing)
        {
            _context.ListingAds.Update(listing);
            if (listing.Textbook != null)
            {
                _context.Textbooks.Update(listing.Textbook);
            }
            await _context.SaveChangesAsync();
            return listing;
        }

        public async Task DeleteListingAsync(int listingId)
        {
            var listing = await _context.ListingAds.FindAsync(listingId);
            if (listing != null)
            {
                _context.ListingAds.Remove(listing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ListingAd>> GetActiveBrowseableListingsAsync(string excludeUserId)
        {
            return await _context.ListingAds
                .Where(l => l.IsActive && l.User_ID != excludeUserId)
                .Include(l => l.Textbook)
                .Include(l => l.User)
                .Include(l => l.Genre)
                .OrderByDescending(l => l.Date_Posted)
                .ToListAsync();
        }

        public async Task<List<ListingAd>> GetFilteredListingsAsync(string? searchTitle, string? genreId, string excludeUserId)
        {
            var query = _context.ListingAds
                .Where(l => l.IsActive && l.User_ID != excludeUserId)
                .Include(l => l.Textbook)
                .Include(l => l.User)
                .Include(l => l.Genre)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTitle))
            {
                query = query.Where(l => l.Textbook.Title.Contains(searchTitle) || l.Textbook.Author.Contains(searchTitle));
            }

            if (!string.IsNullOrEmpty(genreId) && int.TryParse(genreId, out int genreIdInt))
            {
                query = query.Where(l => l.Genre_ID == genreIdInt);
            }

            return await query.OrderByDescending(l => l.Date_Posted).ToListAsync();
        }
    }
}
