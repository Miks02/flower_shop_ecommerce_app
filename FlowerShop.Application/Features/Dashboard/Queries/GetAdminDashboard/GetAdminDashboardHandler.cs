using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Domain.Entities.Products;
using Microsoft.AspNetCore.Identity;

namespace FlowerShop.Application.Features.Dashboard.Queries.GetAdminDashboard;

public class GetAdminDashboardHandler(
    IOrderRepository orderRepo,
    IProductRepository productRepo,
    IDelivererRepository delivererRepo,
    UserManager<User> userManager) : IHandler
{
    private const int RecentOrdersCount = 5;
    private const string CustomerRole = "User";

    public async Task<GetAdminDashboardResponse> Handle(CancellationToken ct = default)
    {
        var orderStats = await orderRepo.GetAdminOrderStatsAsync(ct);
        var (todaySales, todayNewOrders) = await orderRepo.GetTodaySalesStatsAsync(ct);
        var recentOrders = await orderRepo.GetRecentOrdersAsync(RecentOrdersCount, ct);
        var availableProducts = await productRepo.CountAvailableProductsAsync(ct);
        var delivererStats = await delivererRepo.GetStatisticsAsync(ct);
        var customers = await userManager.GetUsersInRoleAsync(CustomerRole);

        return new GetAdminDashboardResponse
        {
            TodaySales = todaySales,
            TodayNewOrders = todayNewOrders,
            ActiveOrders = orderStats.ActiveOrders,
            AvailableProducts = availableProducts,
            DeliverersOnDuty = delivererStats.OnDutyCount,
            RegisteredCustomers = customers.Count,
            RecentOrders = recentOrders.Select(o => new AdminRecentOrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerFullName = $"{o.User.FirstName} {o.User.LastName}".Trim(),
                OrderPrice = o.OrderPrice,
                OrderStatus = o.OrderStatus,
                CreatedAt = o.CreatedAt
            }).ToList()
        };
    }
}
