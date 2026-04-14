using FlowerShop.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence.EntityFramework;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        builder.Entity<User>().ToTable("Users");

        builder.Entity<IdentityRole>().ToTable("Roles");

        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
    }
}