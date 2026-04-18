using FlowerShop.Domain.Entities.FlowerTypes;
using FlowerShop.Domain.Entities.Products;

namespace FlowerShop.Domain.Entities.ProductFlowers;

public class ProductFlower
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public int FlowerId { get; set; }
    public Flower Flower { get; set; } = null!;
    
    public int Quantity { get; set; } = 1;
}