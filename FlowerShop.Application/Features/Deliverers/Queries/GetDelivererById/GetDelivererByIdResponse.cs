using FlowerShop.Domain.Entities.Deliverers;

namespace FlowerShop.Application.Features.Deliverers.Queries.GetDelivererById;

public record GetDelivererByIdResponse
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public VehicleType VehicleType { get; set; }
    public DelivererStatus DelivererStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}
