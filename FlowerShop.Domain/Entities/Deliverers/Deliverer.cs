using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.Domain.Entities.Orders;

namespace FlowerShop.Domain.Entities.Deliverers;

public class Deliverer
{
    public string Id { get; set; } = null!;
    public DelivererStatus DelivererStatus { get; set; } = DelivererStatus.Available;
    public VehicleType VehicleType { get; set; }
    
    public User User { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = [];

    public int MinAmountOfProducts()
    {
        return VehicleType switch
        {
            VehicleType.Bicycle => 1,
            VehicleType.Scooter => 3,
            VehicleType.Car => 5,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public bool IsAvailable()
    {
        return DelivererStatus == DelivererStatus.Available;
    }

    public bool IsOnDuty()
    {
        return DelivererStatus == DelivererStatus.OnDuty;
    }
}