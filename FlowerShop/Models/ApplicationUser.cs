using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FlowerShop.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(50)]
    public string FirstName { get; set; } = null!;
    [MaxLength(50)]
    public string LastName { get; set; } = null!;
   
    [MaxLength(200)]
    public string? ImagePath { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;

}