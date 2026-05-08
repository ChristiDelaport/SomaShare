using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SomaShare.Components.Model
{
    public class SomaContext : IdentityDbContext<User>
    {
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Textbook> Textbooks { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<ListingAd> ListingAds { get; set; }
        public DbSet<WantedAd> WantedAds { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ListingAd Relationships
            modelBuilder.Entity<ListingAd>()
                .HasOne(l => l.User)
                .WithMany(u => u.ListingAds)
                .HasForeignKey(l => l.User_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ListingAd>()
                .HasOne(l => l.Textbook)
                .WithMany(t => t.ListingAds)
                .HasForeignKey(l => l.Textbook_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ListingAd>()
                .HasOne(l => l.Genre)
                .WithMany()
                .HasForeignKey(l => l.Genre_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ListingAd>()
                .HasMany(l => l.Offers)
                .WithOne(o => o.ListingAd)
                .HasForeignKey(o => o.ListingAd_ID)
                .OnDelete(DeleteBehavior.Cascade);

            // WantedAd Relationships
            modelBuilder.Entity<WantedAd>()
                .HasOne(w => w.User)
                .WithMany(u => u.WantedAds)
                .HasForeignKey(w => w.User_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WantedAd>()
                .HasOne(w => w.Textbook)
                .WithMany(t => t.WantedAds)
                .HasForeignKey(w => w.Textbook_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WantedAd>()
                .HasOne(w => w.Genre)
                .WithMany()
                .HasForeignKey(w => w.Genre_ID)
                .OnDelete(DeleteBehavior.Restrict);

            // Offer Relationships
            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Buyer)
                .WithMany()
                .HasForeignKey(o => o.Buyer_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Seller)
                .WithMany()
                .HasForeignKey(o => o.Seller_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Transaction)
                .WithOne()
                .HasForeignKey<Offer>(o => o.Transaction_ID)
                .OnDelete(DeleteBehavior.SetNull);

            // Transaction Relationships
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Buyer)
                .WithMany()
                .HasForeignKey(t => t.Buyer_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Seller)
                .WithMany()
                .HasForeignKey(t => t.Seller_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Offer)
                .WithOne()
                .HasForeignKey<Transaction>(t => t.Offer_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasMany(t => t.Reviews)
                .WithOne(r => r.Transaction)
                .HasForeignKey(r => r.Transaction_ID)
                .OnDelete(DeleteBehavior.Cascade);

            // Review Relationships
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.Reviewer_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewee)
                .WithMany()
                .HasForeignKey(r => r.Reviewee_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Textbook>()
                .HasOne(t => t.Genre)
                .WithMany(g => g.Textbooks)
                .HasForeignKey(t => t.Genre_ID)
                .OnDelete(DeleteBehavior.Restrict);

            // Seeds [Genre]
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Genre_ID = 1, Genre_Name = "Computer Science" },
                new Genre { Genre_ID = 2, Genre_Name = "Mathematics" },
                new Genre { Genre_ID = 3, Genre_Name = "Physics" },
                new Genre { Genre_ID = 4, Genre_Name = "Chemistry" },
                new Genre { Genre_ID = 5, Genre_Name = "Biology" },
                new Genre { Genre_ID = 6, Genre_Name = "History" },
                new Genre { Genre_ID = 7, Genre_Name = "Literature" },
                new Genre { Genre_ID = 8, Genre_Name = "Economics" },
                new Genre { Genre_ID = 9, Genre_Name = "Business" },
                new Genre { Genre_ID = 10, Genre_Name = "Psychology" }
            );
        }

        public SomaContext(DbContextOptions<SomaContext> options) : base(options)
        {
        }
    }
}