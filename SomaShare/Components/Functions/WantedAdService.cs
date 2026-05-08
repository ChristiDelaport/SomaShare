using Microsoft.EntityFrameworkCore;
using SomaShare.Components.Model;

namespace SomaShare.Services
{
    public interface IWantedAdService
    {
        Task<List<WantedAd>> GetUserWantedAdsAsync(string userId);
        Task<WantedAd?> GetWantedAdAsync(int wantedAdId);
        Task<WantedAd> CreateWantedAdAsync(Textbook textbook, WantedAd wantedAd);
        Task<WantedAd> UpdateWantedAdAsync(WantedAd wantedAd);
        Task DeleteWantedAdAsync(int wantedAdId);
        Task<List<WantedAd>> GetActiveBrowseableWantedAdsAsync(string excludeUserId);
        Task<List<WantedAd>> GetFilteredWantedAdsAsync(string? searchTitle, string? genreId, string excludeUserId);
    }

    public class WantedAdService : IWantedAdService
    {
        private readonly SomaContext _context;

        public WantedAdService(SomaContext context)
        {
            _context = context;
        }

        public async Task<List<WantedAd>> GetUserWantedAdsAsync(string userId)
        {
            return await _context.WantedAds
                .Where(w => w.User_ID == userId)
                .Include(w => w.Textbook)
                .Include(w => w.Genre)
                .OrderByDescending(w => w.Date_Posted)
                .ToListAsync();
        }

        public async Task<WantedAd?> GetWantedAdAsync(int wantedAdId)
        {
            return await _context.WantedAds
                .Include(w => w.Textbook)
                .Include(w => w.User)
                .Include(w => w.Genre)
                .FirstOrDefaultAsync(w => w.WantedAd_ID == wantedAdId);
        }

        public async Task<WantedAd> CreateWantedAdAsync(Textbook textbook, WantedAd wantedAd)
        {
            await _context.Textbooks.AddAsync(textbook);
            await _context.SaveChangesAsync();

            wantedAd.Textbook_ID = textbook.Textbook_ID;
            await _context.WantedAds.AddAsync(wantedAd);
            await _context.SaveChangesAsync();

            return wantedAd;
        }

        public async Task<WantedAd> UpdateWantedAdAsync(WantedAd wantedAd)
        {
            _context.WantedAds.Update(wantedAd);
            if (wantedAd.Textbook != null)
            {
                _context.Textbooks.Update(wantedAd.Textbook);
            }
            await _context.SaveChangesAsync();
            return wantedAd;
        }

        public async Task DeleteWantedAdAsync(int wantedAdId)
        {
            var wantedAd = await _context.WantedAds.FindAsync(wantedAdId);
            if (wantedAd != null)
            {
                _context.WantedAds.Remove(wantedAd);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<WantedAd>> GetActiveBrowseableWantedAdsAsync(string excludeUserId)
        {
            return await _context.WantedAds
                .Where(w => w.IsActive && w.User_ID != excludeUserId)
                .Include(w => w.Textbook)
                .Include(w => w.User)
                .Include(w => w.Genre)
                .OrderByDescending(w => w.Date_Posted)
                .ToListAsync();
        }

        public async Task<List<WantedAd>> GetFilteredWantedAdsAsync(string? searchTitle, string? genreId, string excludeUserId)
        {
            var query = _context.WantedAds
                .Where(w => w.IsActive && w.User_ID != excludeUserId);

            if (!string.IsNullOrWhiteSpace(searchTitle))
            {
                query = query.Where(w => w.Textbook.Title.Contains(searchTitle) || w.Textbook.Author.Contains(searchTitle));
            }

            if (!string.IsNullOrWhiteSpace(genreId) && int.TryParse(genreId, out int genreIdInt))
            {
                query = query.Where(w => w.Genre_ID == genreIdInt);
            }

            return await query
                .Include(w => w.Textbook)
                .Include(w => w.User)
                .Include(w => w.Genre)
                .OrderByDescending(w => w.Date_Posted)
                .ToListAsync();
        }
    }
}
