using FlowerShop.Domain.Entities.Orders;

namespace FlowerShop.Web.Areas.Admin.Models.Dashboard;

public record AdminDashboardViewModel
{
    public decimal TodaySales { get; init; }
    public int TodayNewOrders { get; init; }
    public int ActiveOrders { get; init; }
    public int AvailableProducts { get; init; }
    public int DeliverersOnDuty { get; init; }
    public int RegisteredCustomers { get; init; }
    public IReadOnlyList<AdminRecentOrderViewModel> RecentOrders { get; init; } = [];
}

public record AdminRecentOrderViewModel
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = null!;
    public string CustomerFullName { get; init; } = null!;
    public decimal OrderPrice { get; init; }
    public OrderStatus OrderStatus { get; init; }
    public DateTime CreatedAt { get; init; }
}
