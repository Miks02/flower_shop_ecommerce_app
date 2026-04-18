using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Domain.Entities.ProductFlowers;
using FlowerShop.Domain.Entities.Products;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence.EntityFramework;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
    
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductFlower> ProductFlowers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Flower> Flowers { get; set; }
    public DbSet<Occasion> Occasions { get; set; }
    
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