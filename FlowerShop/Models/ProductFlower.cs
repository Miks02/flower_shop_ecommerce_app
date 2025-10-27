namespace FlowerShop.Models;

public class ProductFlower
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int FlowerId { get; set; }
    public FlowerType FlowerType { get; set; } = null!;
}