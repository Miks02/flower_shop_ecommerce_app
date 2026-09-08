using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.Domain.Entities.Products;

namespace FlowerShop.Domain.Entities.ProductReviews;

public class ProductReview
{
    public int Id { get; set; }

    public decimal Rating { get; set; }
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Product Product { get; set; } = null!;
    public int ProductId { get; set; }
    public User User { get; set; } = null!;
    public string ReviewerId { get; set; } = null!;
}