using Microsoft.AspNetCore.Identity;
using ASP_NET_MVC_TEMPLATE8.Models;

namespace ASP_NET_MVC_TEMPLATE8.Helpers;

public static class Seeder
{
    public static async Task SeedUsers(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
   
        string[] roleNames = { "Admin", "CinemaStaff", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        var adminEmail = "admin@gmail.com";
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

        var staffEmail = "cinema@gmail.com";
        if (await userManager.FindByEmailAsync(staffEmail) == null)
        {
            var staffUser = new ApplicationUser
            {
                UserName = "Bioskopski_radnik",
                Email = staffEmail,
                FirstName = "Marija",
                LastName = "Stojanović",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(staffUser, "123456");
            await userManager.AddToRoleAsync(staffUser, "CinemaStaff");
        }

        var userEmail = "user@gmail.com";
        if (await userManager.FindByEmailAsync(userEmail) == null)
        {
            var normalUser = new ApplicationUser
            {
                UserName = "user",
                Email = userEmail,
                FirstName = "FirstName",
                LastName = "LastName",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(normalUser, "123456");
            await userManager.AddToRoleAsync(normalUser, "User");
        } 
    }
}