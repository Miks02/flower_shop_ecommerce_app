using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Orders.Queries.GetDelivererOrders;

public class GetDelivererOrdersResponse
{
    public PagedResult<DelivererOrderDto> PagedOrders { get; init; } = null!;
    public int TotalOrders { get; init; }
    public int PendingOrders { get; init; }
    public int UnassignedOrders { get; init; }
    public int InDeliveryOrders { get; init; }
    public int CompletedOrders { get; init; }
}