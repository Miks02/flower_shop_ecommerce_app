using FlowerShop.Domain.Entities.ProductFlowers;
using FlowerShop.Domain.Enums;

namespace FlowerShop.Domain.Entities.FlowerTypes;

public class FlowerType
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;

    public int Stock { get; set; }
    
    public string Color {get; set; } = null!;
    
    public FlowerCategory FlowerCategory { get; set; } 
    public ICollection<ProductFlower> ProductFlowers { get; set; } = new List<ProductFlower>();
}