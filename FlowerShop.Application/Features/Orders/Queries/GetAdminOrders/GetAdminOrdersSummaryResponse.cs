using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Orders.Queries.GetAdminOrders;

public record GetAdminOrdersSummaryResponse
{
    public PagedResult<AdminOrderDto> PagedOrders { get; init; } = null!;
    public int TotalOrders { get; init; }
    public int PendingOrders { get; init; }
    public int UnassignedOrders { get; init; }
    public int InDeliveryOrders { get; init; }
    public int CompletedOrders { get; init; }
}
