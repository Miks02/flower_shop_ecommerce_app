using FlowerShop.Domain.Entities.Deliverers;

namespace FlowerShop.Web.Areas.Admin.Models.Deliverers;

public record DelivererFormViewModel
{
    public string? Id { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string PhoneNumber { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public VehicleType VehicleType { get; init; } = VehicleType.Bicycle;

    public DelivererStatus DelivererStatus { get; init; } = DelivererStatus.Available;

    public DateTime? CreatedAt { get; init; }
}
