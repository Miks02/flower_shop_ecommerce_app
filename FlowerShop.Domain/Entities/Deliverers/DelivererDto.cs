namespace FlowerShop.Domain.Entities.Deliverers;

public record DelivererDto
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
