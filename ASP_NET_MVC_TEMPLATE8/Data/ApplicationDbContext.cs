using ASP_NET_MVC_TEMPLATE8.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ASP_NET_MVC_TEMPLATE8.Data;

public class ApplicationDbContext :  IdentityDbContext<ApplicationUser>
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
     protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            

            var userRoleId = "dbab9655-2b2a-480d-bff5-2dbdc1df620b";
            var adminRoleId = "86b70d9b-bc3c-4008-aab7-0b3d0ceb5d0d";


            var userId = "593a1b11-ca84-4cac-b672-91193cdf5cdb";
            var adminId = "b14ed7b5-a440-4854-9bd1-a68f7b943edb";



            var roles = new List<IdentityRole>
             {
                 new IdentityRole
                 {
                     Name = "Admin",
                     NormalizedName = "ADMIN",
                     Id = adminRoleId,
                     ConcurrencyStamp = adminRoleId
                 },

               

                 new IdentityRole
                 {
                     Name = "User",
                     NormalizedName = "USER",
                     Id = userRoleId,
                     ConcurrencyStamp = userRoleId
                 },

             };

            builder.Entity<IdentityRole>().HasData(roles);


            var userRoles = new List<IdentityUserRole<string>>
             {
                 new IdentityUserRole<string>
                 {
                     RoleId = userRoleId,
                     UserId = userId
                 },
           
                 new IdentityUserRole<string>
                 {
                     RoleId = adminRoleId,
                     UserId = adminId
                 },

             };





        }
}