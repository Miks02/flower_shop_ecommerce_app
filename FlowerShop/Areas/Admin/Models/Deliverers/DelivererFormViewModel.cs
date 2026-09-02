using FlowerShop.Domain.Entities.Deliverers;

namespace FlowerShop.Web.Areas.Admin.Models.Deliverers;

public record DelivererFormViewModel
{
    public string? Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public VehicleType VehicleType { get; set; } = VehicleType.Bicycle;

    public DelivererStatus DelivererStatus { get; set; } = DelivererStatus.Available;

    public DateTime? CreatedAt { get; set; }
}
