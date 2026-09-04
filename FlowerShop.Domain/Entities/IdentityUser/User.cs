using FlowerShop.Domain.Entities.Carts;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.Domain.Enums;

namespace FlowerShop.Domain.Entities.IdentityUser;

public class User : Microsoft.AspNetCore.Identity.IdentityUser
{
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
   
    public string? ImagePath { get; set; }

    public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public ICollection<Product> Products { get; set; } = new List<Product>();

    public Deliverer? Deliverer { get; set; }
    public Cart? Cart { get; set; }
}