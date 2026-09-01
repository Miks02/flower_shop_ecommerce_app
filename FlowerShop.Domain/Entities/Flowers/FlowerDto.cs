using FlowerShop.Domain.Enums;

namespace FlowerShop.Domain.Entities.Flowers;

public record FlowerDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;

    public int Stock { get; set; }
    
    public string Color {get; set; } = null!;
    
    public FlowerCategory FlowerCategory { get; set; } 
};