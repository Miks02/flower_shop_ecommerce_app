using System.Reflection;
using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace FlowerShop.Data;

public class ApplicationDbContext :  IdentityDbContext<ApplicationUser>
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<FlowerType> FlowerTypes { get; set; }
    public DbSet<Occasion> Occasions { get; set; }
    public DbSet<ProductFlower> ProductFlowers { get; set; }
    
     protected override void OnModelCreating(ModelBuilder builder)
     {
         builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
         base.OnModelCreating(builder);
            
         builder.Entity<ApplicationUser>()
             .Property(u => u.PhoneNumber)
             .IsRequired();   

        }
}