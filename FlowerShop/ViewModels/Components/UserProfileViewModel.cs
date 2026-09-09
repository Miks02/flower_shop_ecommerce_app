using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Domain.Enums;

namespace FlowerShop.Web.ViewModels.Components;

public class UserProfileViewModel
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? ProfilePicture { get; set; }
    
    public AccountStatus Status { get; set; }

    public string RegistrationDate { get; set; } = null!;

    public int TotalOrders { get; set; } = 0;

    public int LoyaltyPoints { get; set; } = 0;

    public int UpcomingOrders { get; set; } = 0;

    public IReadOnlyList<UserRecentOrderViewModel> RecentOrders { get; set; } = [];
}

public record UserRecentOrderViewModel
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = null!;
    public decimal OrderPrice { get; init; }
    public OrderStatus OrderStatus { get; init; }
    public DateTime CreatedAt { get; init; }
}