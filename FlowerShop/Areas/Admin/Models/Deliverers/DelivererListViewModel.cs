using FlowerShop.Domain.Entities.Deliverers;

namespace FlowerShop.Web.Areas.Admin.Models.Deliverers;

public record DelivererListViewModel
{
    public string Id { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
    public string Email { get; init; } = null!;
    public VehicleType VehicleType { get; init; }
    public DelivererStatus DelivererStatus { get; init; }
    public DateTime CreatedAt { get; init; }
    public int MinAmountOfProducts => VehicleType switch
    {
        VehicleType.Bicycle => 1,
        VehicleType.Scooter => 3,
        VehicleType.Car => 5,
        _ => 1
    };
}