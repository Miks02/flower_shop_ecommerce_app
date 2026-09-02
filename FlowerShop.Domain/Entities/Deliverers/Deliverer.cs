using FlowerShop.Domain.Entities.IdentityUser;

namespace FlowerShop.Domain.Entities.Deliverers;

public class Deliverer
{
    public string Id { get; set; } = null!;
    public DelivererStatus DelivererStatus { get; set; } = DelivererStatus.Available;
    public VehicleType VehicleType { get; set; }
    public int MaxAmountOfOrders { get; set; }
    
    public User User { get; set; } = null!;
}