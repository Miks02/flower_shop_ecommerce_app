using FlowerShop.Domain.Entities.IdentityUser;
using Microsoft.AspNetCore.Identity;


namespace FlowerShop.Web.Helpers;

public static class Seeder
{
    public static async Task SeedUsers(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
   
        string[] roleNames = { "Admin", "Deliverer", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        var adminEmail = configuration["AdminData:Email"];

        if (string.IsNullOrWhiteSpace(adminEmail))
            throw new ArgumentNullException(nameof(adminEmail), "Pass in a valid email address for an administrator");
        
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var adminUser = new User
            {
                UserName = configuration["AdminData:Username"],
                Email = adminEmail,
                FirstName = configuration["AdminData:FirstName"]!,
                LastName = configuration["AdminData:LastName"]!,
                PhoneNumber = configuration["AdminData:PhoneNumber"]!,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, configuration["AdminData:Password"]!);
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

       
    }
}