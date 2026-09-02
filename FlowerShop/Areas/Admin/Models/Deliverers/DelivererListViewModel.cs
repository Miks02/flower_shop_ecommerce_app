using FlowerShop.Domain.Entities.Deliverers;

namespace FlowerShop.Web.Areas.Admin.Models.Deliverers;

public record DelivererListViewModel
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public VehicleType VehicleType { get; set; }
    public DelivererStatus DelivererStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MinAmountOfProducts => VehicleType switch
    {
        VehicleType.Bicycle => 1,
        VehicleType.Scooter => 3,
        VehicleType.Car => 5,
        _ => 1
    };
}