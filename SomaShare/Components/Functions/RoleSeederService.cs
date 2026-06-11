using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SomaShare.Components.Model;

namespace SomaShare.Services
{
    // CHANGE - Combined role + data seeder service. This consolidates the previous
    // DataSeederService logic into a single RoleSeederService so the app
    // only has to resolve and call one seeder at startup.
    public class RoleSeederService
    {
        private readonly SomaContext context;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<RoleSeederService> logger;

        public RoleSeederService(SomaContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger<RoleSeederService> logger)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
        }

        // Public method called from Program.cs at startup to seed roles and initial data
        public async Task SeedAsync()
        {
            logger.LogInformation("RoleSeeder: starting seed process");

            // Apply migrations if available
            try
            {
                await context.Database.MigrateAsync();
                logger.LogInformation("RoleSeeder: migrations applied (if any)");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "RoleSeeder: failed to apply migrations; continuing");
            }

            // Ensure roles exist
            var roles = new[] { "Admin", "Seller", "Buyer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation("RoleSeeder: created role {role}", role);
                }
            }

            // Seed genres
            if (!context.Genres.Any())
            {
                context.Genres.AddRange(
                    new Genre { Genre_Name = "Science" },
                    new Genre { Genre_Name = "Mathematics" },
                    new Genre { Genre_Name = "History" },
                    new Genre { Genre_Name = "Computer Science" }
                );
                await context.SaveChangesAsync();
            }

            // Seed users
            if (!context.Users.Any())
            {
                logger.LogInformation("RoleSeeder: seeding users");

                var buyer = new User
                {
                    UserName = "alice@gmail.com",
                    Email = "alice@gmail.com",
                    First_Name = "Alice",
                    Last_Name = "Buyer",
                    Password = "P@ssw0rd1",
                    EnrollmentDate = DateTime.Now,
                    User_Id = 1,
                    Role_Id = 0
                };

                var seller = new User
                {
                    UserName = "bob@gmail.com",
                    Email = "bob@gmail.com",
                    First_Name = "Bob",
                    Last_Name = "Seller",
                    Password = "P@ssw0rd1",
                    EnrollmentDate = DateTime.Now,
                    User_Id = 2,
                    Role_Id = 2
                };

                var admin = new User
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    First_Name = "Site",
                    Last_Name = "Admin",
                    Password = "P@ssw0rd1",
                    EnrollmentDate = DateTime.Now,
                    User_Id = 3,
                    Role_Id = 3
                };

                var existingBuyer = await userManager.FindByEmailAsync(buyer.Email);
                if (existingBuyer == null)
                {
                    var result = await userManager.CreateAsync(buyer, "P@ssw0rd1");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(buyer, "Buyer");
                        logger.LogInformation("RoleSeeder: created buyer {email}", buyer.Email);
                    }
                }

                var existingSeller = await userManager.FindByEmailAsync(seller.Email);
                if (existingSeller == null)
                {
                    var result = await userManager.CreateAsync(seller, "P@ssw0rd1");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(seller, "Seller");
                        logger.LogInformation("RoleSeeder: created seller {email}", seller.Email);
                    }
                }

                var existingAdmin = await userManager.FindByEmailAsync(admin.Email);
                if (existingAdmin == null)
                {
                    var result = await userManager.CreateAsync(admin, "P@ssw0rd1");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                        logger.LogInformation("RoleSeeder: created admin {email}", admin.Email);
                    }
                }

                await context.SaveChangesAsync();
            }

            logger.LogInformation("RoleSeeder: ensuring textbooks and listing ads");
            // Seed textbooks and listing ads
            if (!context.Textbooks.Any())
            {
                var csGenre = context.Genres.FirstOrDefault(g => g.Genre_Name == "Computer Science") ?? context.Genres.First();

                var tb1 = new Textbook
                {
                    Title = "Introduction to Algorithms",
                    Author = "Cormen, Leiserson, Rivest, Stein",
                    ISBN = "9780262033848",
                    Version = "3rd",
                    Genre_Id = csGenre.Genre_Id,
                    Description = "Classic algorithms textbook"
                };

                var tb2 = new Textbook
                {
                    Title = "Clean Code",
                    Author = "Robert C. Martin",
                    ISBN = "9780132350884",
                    Version = "1st",
                    Genre_Id = csGenre.Genre_Id,
                    Description = "A Handbook of Agile Software Craftsmanship"
                };

                context.Textbooks.AddRange(tb1, tb2);
                await context.SaveChangesAsync();

                var sellerUser = await userManager.FindByEmailAsync("bob@gmail.com");
                if (sellerUser != null)
                {
                    var listing = new ListingAd
                    {
                        User_Id = sellerUser.Id,
                        Textbook_Id = tb1.Textbook_Id,
                        Price = 50.00m,
                        Condition = "Good",
                        Date_Posted = DateTime.Now,
                        IsActive = true,
                        Genre_Id = tb1.Genre_Id
                    };

                    context.ListingAds.Add(listing);
                    await context.SaveChangesAsync();
                    logger.LogInformation("RoleSeeder: created listing for {email}", sellerUser.Email);
                }
            }

            logger.LogInformation("RoleSeeder: ensuring wanted ads");
            // Seed wanted ads
            if (!context.WantedAds.Any())
            {
                var buyerUser = await userManager.FindByEmailAsync("alice@gmail.com");
                var genre = context.Genres.FirstOrDefault() ?? context.Genres.First();

                var wantedTextbook = new Textbook
                {
                    Title = "Discrete Mathematics",
                    Author = "Kenneth Rosen",
                    ISBN = "",
                    Version = "",
                    Genre_Id = genre.Genre_Id,
                    Description = "Looking for a used copy"
                };

                context.Textbooks.Add(wantedTextbook);
                await context.SaveChangesAsync();

                if (buyerUser != null)
                {
                    var wanted = new WantedAd
                    {
                        User_Id = buyerUser.Id,
                        Textbook_Id = wantedTextbook.Textbook_Id,
                        Genre_Id = wantedTextbook.Genre_Id,
                        Date_Posted = DateTime.Now,
                        IsActive = true,
                        MaxPrice = 30.00m
                    };

                    context.WantedAds.Add(wanted);
                    await context.SaveChangesAsync();
                    logger.LogInformation("RoleSeeder: created wanted ad for {email}", buyerUser.Email);
                }
            }

            logger.LogInformation("RoleSeeder: seed process complete");
        }
    }
}
