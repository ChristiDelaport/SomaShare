using Microsoft.AspNetCore.Identity;
using SomaShare.Components.Model;

namespace SomaShare.Services
{

    public class RoleSeederService
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleSeederService(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        public async Task SeedRolesAsync()
        {
            // Define roles to create
            var roles = new[]
            {
                "Admin",
                "Seller",
                "Buyer"
            };

            // Create each role if it doesn't exist
            foreach (var roleName in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    Console.WriteLine("Created Sucessfully");
                }
                else
                {
                    Console.WriteLine("Already Created");
                }
            }
        }
    }
}
