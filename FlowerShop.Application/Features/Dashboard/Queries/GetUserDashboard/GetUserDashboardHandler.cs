using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.LoyaltyTransactions;
using FlowerShop.Domain.Entities.Orders;

namespace FlowerShop.Application.Features.Dashboard.Queries.GetUserDashboard;

public class GetUserDashboardHandler(
    IOrderRepository orderRepo,
    ILoyaltyTransactionRepository loyaltyRepo) : IHandler
{
    private const int RecentOrdersCount = 5;

    public async Task<GetUserDashboardResponse> Handle(GetUserDashboardQuery query, CancellationToken ct = default)
    {
        var orderStats = await orderRepo.GetUserOrderStatsAsync(query.UserId, ct);
        var loyaltyPoints = await loyaltyRepo.GetCurrentLoyaltyPoints(query.UserId, ct);
        var recentOrders = await orderRepo.GetRecentOrdersForUserAsync(query.UserId, RecentOrdersCount, ct);

        return new GetUserDashboardResponse
        {
            TotalOrders = orderStats.TotalOrders,
            UpcomingOrders = orderStats.PendingOrders + orderStats.InDeliveryOrders,
            LoyaltyPoints = loyaltyPoints,
            RecentOrders = recentOrders.Select(o => new UserRecentOrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderPrice = o.OrderPrice,
                OrderStatus = o.OrderStatus,
                CreatedAt = o.CreatedAt
            }).ToList()
        };
    }
}
