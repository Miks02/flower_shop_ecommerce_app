using FlowerShop.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace FlowerShop.Domain.Entities.Users;

public class User : IdentityUser
{
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
   
    public string? ImagePath { get; set; }

    public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;

}