using Microsoft.AspNetCore.Identity;
using UsersService.Domain.Entities;

namespace UsersService.API.Helpers
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            
            string[] roleNames = {"Admin", "Client" };
            
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
        
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            
            if (await userManager.FindByEmailAsync("admin@flowerlab.com") == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@flowerlab.com",
                    Email = "admin@flowerlab.com",
                    FirstName = "Tanya",
                    PhoneNumber = "+380957778899",
                    LastName = "FlowerLAB",
                    PhotoUrl = "https://res.cloudinary.com/dg9clyn4k/image/upload/v1763712578/order-service/gifts/mpfiss97mfebcqwm6elb.jpg",
                    EmailConfirmed = true,
                };
                
                await userManager.CreateAsync(adminUser, "SecureAdminPass123!"); 
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            if (await userManager.FindByEmailAsync("client@flowerlab.com") == null)
            {
                var clientUser = new ApplicationUser
                {
                    UserName = "client@flowerlab.com",
                    Email = "client@flowerlab.com",
                    FirstName = "Ivan",
                    LastName = "Petrov",
                    PhoneNumber = "+380501112233",
                    PhotoUrl = "https://res.cloudinary.com/dg9clyn4k/image/upload/v1763712578/order-service/gifts/default-user.jpg",
                    EmailConfirmed = true,
                };

                await userManager.CreateAsync(clientUser, "ClientPass123!");
                await userManager.AddToRoleAsync(clientUser, "Client");
            }
        }
    }
}