using System.ComponentModel.DataAnnotations;
using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Domain.Entities.ProductFlowers;

namespace FlowerShop.Domain.Entities.Products;

public class Product
{
    public int Id { get; set; }
    
    [StringLength(50)]
    public string Name { get; set; } = null!;
    
    [StringLength(750)]
    public string? Description { get; set; }
    
    [StringLength(750)]
    public string ImageUrl { get; set; } = null!;
    
    [Range(0.01, (double) decimal.MaxValue)]
    public decimal Price { get; set; }
    
    [Range(0.01, (double) decimal.MaxValue)]
    public decimal? PromoPrice { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    public ICollection<Occasion> Occasions { get; set; } = new List<Occasion>();
    public ICollection<ProductFlower> ProductFlowers { get; set; } = new List<ProductFlower>();
    
}