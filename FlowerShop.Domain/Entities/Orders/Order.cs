using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.IdentityUser;

namespace FlowerShop.Domain.Entities.Orders;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = GenerateOrderNumber();

    public string RecipientFullName { get; set; } = null!;
    public string RecipientPhoneNumber { get; set; } = null!;
    public string OrderAddress { get; set; } = null!;
    public string? Note { get; set; }
    public string City { get; set; } = null!;
    public string ZipCode { get; set; } = null!;

    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public DeliveryStatus DeliveryStatus { get; set; } = DeliveryStatus.Standby;
    
    public DateTime OrderDate { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public string UserId { get; set; } = null!;
    
    public Deliverer? Deliverer { get; set; }
    public string? DelivererId { get; set; }
    
    public ICollection<OrderItem> OrderItems { get; set; } = [];
    
    public decimal OrderPrice => OrderItems.Sum(oi => oi.TotalPrice);

    private static string GenerateOrderNumber()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = Guid.NewGuid().ToString("N")[..5].ToUpper(); 
    
        return $"ORD-{datePart}-{randomPart}";
    }
}