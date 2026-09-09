using FlowerShop.Domain.Entities.Orders;

namespace FlowerShop.Application.Features.Dashboard.Queries.GetUserDashboard;

public record GetUserDashboardResponse
{
    public int TotalOrders { get; init; }
    public int UpcomingOrders { get; init; }
    public int LoyaltyPoints { get; init; }
    public IReadOnlyList<UserRecentOrderDto> RecentOrders { get; init; } = [];
}

public record UserRecentOrderDto
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = null!;
    public decimal OrderPrice { get; init; }
    public OrderStatus OrderStatus { get; init; }
    public DateTime CreatedAt { get; init; }
}
