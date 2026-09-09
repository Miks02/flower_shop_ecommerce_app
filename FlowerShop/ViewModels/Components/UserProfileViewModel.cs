using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Domain.Enums;

namespace FlowerShop.Web.ViewModels.Components;

public record UserProfileViewModel
{
    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;

    public string Email { get; init; } = null!;

    public string PhoneNumber { get; init; } = null!;

    public string? ProfilePicture { get; init; }

    public AccountStatus Status { get; init; }

    public string RegistrationDate { get; init; } = null!;

    public int TotalOrders { get; init; } = 0;

    public int LoyaltyPoints { get; init; } = 0;

    public int UpcomingOrders { get; init; } = 0;

    public IReadOnlyList<UserRecentOrderViewModel> RecentOrders { get; init; } = [];
}

public record UserRecentOrderViewModel
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = null!;
    public decimal OrderPrice { get; init; }
    public OrderStatus OrderStatus { get; init; }
    public DateTime CreatedAt { get; init; }
}