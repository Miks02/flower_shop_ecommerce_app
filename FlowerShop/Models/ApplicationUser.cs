using System.ComponentModel.DataAnnotations;
using FlowerShop.Web.Enums;
using Microsoft.AspNetCore.Identity;

namespace FlowerShop.Web.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(50)]
    public string FirstName { get; set; } = null!;
    
    [MaxLength(50)]
    public string LastName { get; set; } = null!;
   
    [MaxLength(200)]
    public string? ImagePath { get; set; }

    public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;

}