using Microsoft.EntityFrameworkCore;
using SomaShare.Components.Model;

namespace SomaShare.Services
{
    public interface IReviewService
    {
        Task<double> GetAverageRatingAsync(string userId);

        Task<Review?> GetReviewAsync(int reviewId);
        Task<List<Review>> GetReviewsForUserAsync(string userId);
        Task<List<Review>> GetReviewsByReviewerAsync(string reviewerId);
        Task<Review> CreateReviewAsync(Review review);
        Task<Review> UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(int reviewId);
        Task<bool> UserHasReviewedTransactionAsync(int transactionId, string reviewerId);
        Task<decimal> GetUserAverageRatingAsync(string userId);
    }

    public class ReviewService : IReviewService
    {
        private readonly SomaContext _context;

        public ReviewService(SomaContext context)
        {
            _context = context;
        }

        public async Task<Review?> GetReviewAsync(int reviewId)
        {
            return await _context.Reviews
                .Include(r => r.Transaction)
                .Include(r => r.Reviewer)
                .Include(r => r.Reviewee)
                .FirstOrDefaultAsync(r => r.Review_ID == reviewId);
        }

        public async Task<List<Review>> GetReviewsForUserAsync(string userId)
        {
            return await _context.Reviews
                .Where(r => r.Reviewee_ID == userId)
                .Include(r => r.Reviewer)
                .Include(r => r.Transaction)
                .OrderByDescending(r => r.Date_Created)
                .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByReviewerAsync(string reviewerId)
        {
            return await _context.Reviews
                .Where(r => r.Reviewer_ID == reviewerId)
                .Include(r => r.Reviewee)
                .Include(r => r.Transaction)
                .OrderByDescending(r => r.Date_Created)
                .ToListAsync();
        }

        public async Task<Review> CreateReviewAsync(Review review)
        {
            review.Date_Created = DateTime.UtcNow;
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review> UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UserHasReviewedTransactionAsync(int transactionId, string reviewerId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.Transaction_ID == transactionId && r.Reviewer_ID == reviewerId);
        }

        public async Task<decimal> GetUserAverageRatingAsync(string userId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.Reviewee_ID == userId)
                .ToListAsync();

            if (reviews.Count == 0)
                return 0;

            return (decimal)reviews.Average(r => r.Rating);
        }

        public async Task<double> GetAverageRatingAsync(string userId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.Reviewee_ID == userId)
                .ToListAsync();

            if (reviews.Count == 0)
                return 0; // no reviews yet

            return reviews.Average(r => r.Rating);
        }

    }
}
