namespace FlowerShop.Domain.Entities.Orders;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string? ProductImagePath { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    public decimal TotalPrice => Quantity * UnitPrice;
}