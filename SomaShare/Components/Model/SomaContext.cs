using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
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
                .HasForeignKey(l => l.User_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ListingAd>()
                .HasOne(l => l.Textbook)
                .WithMany(t => t.ListingAds)
                .HasForeignKey(l => l.Textbook_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ListingAd>()
                .HasOne(l => l.Genre)
                .WithMany()
                .HasForeignKey(l => l.Genre_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ListingAd>()
                .HasMany(l => l.Offers)
                .WithOne(o => o.ListingAd)
                .HasForeignKey(o => o.ListingAd_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // WantedAd Relationships
            modelBuilder.Entity<WantedAd>()
                .HasOne(w => w.User)
                .WithMany(u => u.WantedAds)
                .HasForeignKey(w => w.User_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WantedAd>()
                .HasOne(w => w.Textbook)
                .WithMany(t => t.WantedAds)
                .HasForeignKey(w => w.Textbook_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WantedAd>()
                .HasOne(w => w.Genre)
                .WithMany()
                .HasForeignKey(w => w.Genre_Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Offer Relationships
            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Buyer)
                .WithMany()
                .HasForeignKey(o => o.Buyer_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Seller)
                .WithMany()
                .HasForeignKey(o => o.Seller_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Transaction)
                .WithOne()
                .HasForeignKey<Offer>(o => o.Transaction_Id)
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
                .HasForeignKey(r => r.Transaction_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Review Relationships
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.Reviewer_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewee)
                .WithMany()
                .HasForeignKey(r => r.Reviewee_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Textbook>()
                .HasOne(t => t.Genre)
                .WithMany(g => g.Textbooks)
                .HasForeignKey(t => t.Genre_Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Seeds [Genre]
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Genre_Id = 1, Genre_Name = "Computer Science" },
                new Genre { Genre_Id = 2, Genre_Name = "Mathematics" },
                new Genre { Genre_Id = 3, Genre_Name = "Physics" },
                new Genre { Genre_Id = 4, Genre_Name = "Chemistry" },
                new Genre { Genre_Id = 5, Genre_Name = "Biology" },
                new Genre { Genre_Id = 6, Genre_Name = "History" },
                new Genre { Genre_Id = 7, Genre_Name = "Literature" },
                new Genre { Genre_Id = 8, Genre_Name = "Economics" },
                new Genre { Genre_Id = 9, Genre_Name = "Business" },
                new Genre { Genre_Id = 10, Genre_Name = "Psychology" }
            );
            // Seed Users - compute password hashes so Identity can validate seeded credentials


            var u1 = new User
            {
                Id = "1",
                User_Id = 1,
                UserName = "johnsmith",
                NormalizedUserName = "JOHNSMITH",
                Email = "john@UCT.co.za",
                NormalizedEmail = "JOHN@UCT.CO.ZA",
                First_Name = "John",
                Last_Name = "Smith",
                EmailConfirmed = true,
                Password = "Rand0mPassword1",
                Role_ID = 1,
                EnrollmentDate = DateTime.Now
            };

            var u2 = new User
            {
                Id = "2",
                User_Id = 2,
                UserName = "sarahjones",
                NormalizedUserName = "SARAHJONES",
                Email = "sarah@Stadio.ac.za",
                NormalizedEmail = "SARAH@STADIO.AC.ZA",
                First_Name = "Sarah",
                Last_Name = "Jones",
                EmailConfirmed = true,
                Password = "Rand0mPassword2",
                Role_ID = 1,
                EnrollmentDate = DateTime.Now
            };

            var u3 = new User
            {
                Id = "3",
                User_Id = 3,
                UserName = "michaelbrown",
                NormalizedUserName = "MICHAELBROWN",
                Email = "michael@example.com",
                NormalizedEmail = "MICHAEL@EXAMPLE.COM",
                First_Name = "Michael",
                Last_Name = "Brown",
                EmailConfirmed = true,
                Password = "S33dedPassword!",
                Role_ID = 2,
                EnrollmentDate = DateTime.Now
            };
          

            var u4 = new User
            {
                Id = "4",
                User_Id = 4,
                UserName = "emilydavis",
                NormalizedUserName = "EMILYDAVIS",
                Email = "emily@example.com",
                NormalizedEmail = "EMILY@EXAMPLE.COM",
                First_Name = "Emily",
                Last_Name = "Davis",
                EmailConfirmed = true,
                Password = "AHHH@12C00l",
                Role_ID = 3,
                EnrollmentDate = DateTime.Now
            };

            var u5 = new User
            {
                Id = "5",
                User_Id = 5,
                UserName = "danielwilson",
                NormalizedUserName = "DANIELWILSON",
                Email = "daniel@example.com",
                NormalizedEmail = "DANIEL@EXAMPLE.COM",
                First_Name = "Daniel",
                Last_Name = "Wilson",
                EmailConfirmed = true,
                Password = "SF11SHHH!",
                Role_ID = 2,
                EnrollmentDate = DateTime.Now
            };

            modelBuilder.Entity<User>().HasData(u1, u2, u3, u4, u5); // takes the user obj and seeds into the database

        }

        public SomaContext(DbContextOptions<SomaContext> options) : base(options)
        {
        }
    }
}