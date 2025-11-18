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
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true,
                };
                
                await userManager.CreateAsync(adminUser, "SecureAdminPass123!"); 
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}