using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Models;

public class Product
{
    public int Id { get; set; }
    [StringLength(50)]
    public string Name { get; set; } = null!;
    [StringLength(750)]
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = null!;
    [Range(0.01, (double) decimal.MaxValue)]
    public decimal Price { get; set; }
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<Occasion> Occasions { get; set; } = new List<Occasion>();
    
}