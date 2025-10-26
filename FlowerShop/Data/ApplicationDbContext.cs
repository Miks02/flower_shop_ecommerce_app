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
    
     protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            

            





        }
}