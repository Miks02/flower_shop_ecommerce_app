namespace FlowerShop.Domain.Entities.ProductFlowers;

public record ProductFlowerDto
{
    public int ProductId { get; set; }
    public int FlowerId { get; set; }

    public string ProductName { get; set; } = null!;
    public string FlowerName { get; set; } = null!;
    
    public int Quantity { get; set; }
}