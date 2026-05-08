using Microsoft.EntityFrameworkCore;
using SomaShare.Components.Model;

namespace SomaShare.Services
{
    public interface ITransactionService
    {
        Task<Transaction?> GetTransactionAsync(int transactionId);
        Task<List<Transaction>> GetUserTransactionsAsync(string userId);
        Task<List<Transaction>> GetBuyerTransactionsAsync(string buyerId);
        Task<List<Transaction>> GetSellerTransactionsAsync(string sellerId);
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
        Task<Transaction> CompleteTransactionAsync(int transactionId);
        Task<List<Transaction>> GetPendingTransactionsAsync(string userId);
    }

    public class TransactionService : ITransactionService
    {
        private readonly SomaContext _context;

        public TransactionService(SomaContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetTransactionAsync(int transactionId)
        {
            return await _context.Transactions
                .Include(t => t.Buyer)
                .Include(t => t.Seller)
                .Include(t => t.Offer)
                .ThenInclude(o => o.ListingAd)
                .ThenInclude(l => l.Textbook)
                .Include(t => t.Reviews)
                .FirstOrDefaultAsync(t => t.Transaction_Id == transactionId);
        }

        public async Task<List<Transaction>> GetUserTransactionsAsync(string userId)
        {
            return await _context.Transactions
                .Where(t => t.Buyer_Id == userId || t.Seller_Id == userId)
                .Include(t => t.Buyer)
                .Include(t => t.Seller)
                .Include(t => t.Offer)
                .ThenInclude(o => o.ListingAd)
                .ThenInclude(l => l.Textbook)
                .OrderByDescending(t => t.Date_Created)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetBuyerTransactionsAsync(string buyerId)
        {
            return await _context.Transactions
                .Where(t => t.Buyer_Id == buyerId)
                .Include(t => t.Seller)
                .Include(t => t.Offer)
                .ThenInclude(o => o.ListingAd)
                .ThenInclude(l => l.Textbook)
                .OrderByDescending(t => t.Date_Created)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetSellerTransactionsAsync(string sellerId)
        {
            return await _context.Transactions
                .Where(t => t.Seller_Id == sellerId)
                .Include(t => t.Buyer)
                .Include(t => t.Offer)
                .ThenInclude(o => o.ListingAd)
                .ThenInclude(l => l.Textbook)
                .OrderByDescending(t => t.Date_Created)
                .ToListAsync();
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            transaction.Date_Created = DateTime.UtcNow;
            transaction.Status = "Pending";

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction> CompleteTransactionAsync(int transactionId)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);

            if (transaction == null)
                throw new InvalidOperationException("Transaction not found");

            transaction.Status = "Completed";
            transaction.Date_Completed = DateTime.UtcNow;

            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<List<Transaction>> GetPendingTransactionsAsync(string userId)
        {
            return await _context.Transactions
                .Where(t => (t.Buyer_Id == userId || t.Seller_Id == userId) && t.Status == "Pending")
                .Include(t => t.Buyer)
                .Include(t => t.Seller)
                .Include(t => t.Offer)
                .ThenInclude(o => o.ListingAd)
                .ThenInclude(l => l.Textbook)
                .OrderByDescending(t => t.Date_Created)
                .ToListAsync();
        }
    }
}
