using Bungalov.Core.Varliklar;
using Microsoft.AspNetCore.Identity;

namespace Bungalov.WebUI.Seeds;

public static class DbInitializer
{
    public static async Task SeedRolesAndUserAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        string[] roleNames = { "Admin", "Member" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Admin kullanıcısı oluştur
        var adminUser = await userManager.FindByEmailAsync("admin@bungalov.com");
        if (adminUser == null)
        {
            var admin = new AppUser
            {
                UserName = "admin@bungalov.com",
                Email = "admin@bungalov.com",
                FirstName = "Sistem",
                LastName = "Yöneticisi",
                Address = "Merkez Ofis",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var createAdmin = await userManager.CreateAsync(admin, "Admin123!");
            if (createAdmin.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
