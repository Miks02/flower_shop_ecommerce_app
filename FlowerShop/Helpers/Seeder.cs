using Microsoft.AspNetCore.Identity;
using FlowerShop.Models;

namespace FlowerShop.Helpers;

public static class Seeder
{
    public static async Task SeedUsers(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
   
        string[] roleNames = { "Admin", "Courier", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        var adminEmail = "milan.miki.nikolic2002@gmail.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = "Admin",
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "Admin",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "123456");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

       
    }
}