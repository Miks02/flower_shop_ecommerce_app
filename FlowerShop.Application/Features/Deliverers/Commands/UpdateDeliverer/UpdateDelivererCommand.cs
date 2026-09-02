using FlowerShop.Domain.Entities.Deliverers;

namespace FlowerShop.Application.Features.Deliverers.Commands.UpdateDeliverer;

public record UpdateDelivererCommand
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public VehicleType VehicleType { get; set; }
    public DelivererStatus DelivererStatus { get; set; }
}
