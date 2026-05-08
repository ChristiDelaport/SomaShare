using Microsoft.EntityFrameworkCore;
using SomaShare.Components.Model;

namespace SomaShare.Services
{
    public interface IOfferService
    {
        Task<Offer?> GetOfferAsync(int offerId);
        Task<List<Offer>> GetListingOffersAsync(int listingId);
        Task<List<Offer>> GetBuyerOffersAsync(string buyerId);
        Task<List<Offer>> GetSellerOffersAsync(string sellerId);
        Task<Offer> CreateOfferAsync(Offer offer);
        Task<Offer> UpdateOfferStatusAsync(int offerId, string status);
        Task<int> GetPendingOffersCountAsync(int listingId);
        Task<bool> HasUserMadeOfferAsync(int listingId, string buyerId);
    }

    public class OfferService : IOfferService
    {
        private readonly SomaContext _context;

        public OfferService(SomaContext context)
        {
            _context = context;
        }

        public async Task<Offer?> GetOfferAsync(int offerId)
        {
            return await _context.Offers
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .Include(o => o.ListingAd)
                .ThenInclude(l => l.Textbook)
                .Include(o => o.Transaction)
                .FirstOrDefaultAsync(o => o.Offer_ID == offerId);
        }

        public async Task<List<Offer>> GetListingOffersAsync(int listingId)
        {
            return await _context.Offers
                .Where(o => o.ListingAd_ID == listingId)
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .Include(o => o.ListingAd)
                .OrderByDescending(o => o.DateCreated)
                .ToListAsync();
        }

        public async Task<List<Offer>> GetBuyerOffersAsync(string buyerId)
        {
            return await _context.Offers
                .Where(o => o.Buyer_ID == buyerId)
                .Include(o => o.ListingAd)
                .ThenInclude(l => l.Textbook)
                .Include(o => o.Seller)
                .OrderByDescending(o => o.DateCreated)
                .ToListAsync();
        }

        public async Task<List<Offer>> GetSellerOffersAsync(string sellerId)
        {
            return await _context.Offers
                .Where(o => o.Seller_ID == sellerId)
                .Include(o => o.Buyer)
                .Include(o => o.ListingAd)
                .ThenInclude(l => l.Textbook)
                .OrderByDescending(o => o.DateCreated)
                .ToListAsync();
        }

        public async Task<Offer> CreateOfferAsync(Offer offer)
        {
            offer.DateCreated = DateTime.UtcNow;
            offer.Status = "Pending";

            await _context.Offers.AddAsync(offer);
            await _context.SaveChangesAsync();

            return offer;
        }

        public async Task<Offer> UpdateOfferStatusAsync(int offerId, string status)
        {
            var offer = await _context.Offers.FindAsync(offerId);

            if (offer == null)
                throw new InvalidOperationException("Offer not found");

            offer.Status = status;
            offer.DateResponded = DateTime.UtcNow;

            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();

            return offer;
        }

        public async Task<int> GetPendingOffersCountAsync(int listingId)
        {
            return await _context.Offers
                .Where(o => o.ListingAd_ID == listingId && o.Status == "Pending")
                .CountAsync();
        }

        public async Task<bool> HasUserMadeOfferAsync(int listingId, string buyerId)
        {
            return await _context.Offers
                .AnyAsync(o => o.ListingAd_ID == listingId
                    && o.Buyer_ID == buyerId
                    && (o.Status == "Pending" || o.Status == "Accepted"));
        }
    }
}
