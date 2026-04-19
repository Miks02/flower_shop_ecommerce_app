using FlowerShop.Domain.Entities.ProductFlowers;

namespace FlowerShop.Domain.Entities.Products;

public record ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string ProductImage { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string CreatedBy { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public string CategoryName { get; set; } = null!;
    public IReadOnlyList<string> Occasions { get; set; } = [];
    public IReadOnlyList<ProductFlowerDto> ProductFlowers { get; set; } = [];

}