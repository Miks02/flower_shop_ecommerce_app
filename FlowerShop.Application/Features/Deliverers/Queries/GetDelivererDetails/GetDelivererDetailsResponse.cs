using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Enums;

namespace FlowerShop.Application.Features.Deliverers.Queries.GetDelivererDetails;

public record GetDelivererDetailsResponse
{
    public string DelivererId { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
    public string? ProfilePicture { get; init; }
    public AccountStatus AccountStatus { get; init; }
    public DelivererStatus DelivererStatus { get; init; }
    public VehicleType VehicleType { get; init; }
    public string RegistrationDate { get; init; } = null!;
    public double AverageRating { get; init; } = 4.9;
    public int TotalDeliveries { get; init; }
    public int ActiveDeliveries { get; init; }
    public int CompletedDeliveries { get; init; }
};