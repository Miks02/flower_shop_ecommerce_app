using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Orders.Queries.GetDelivererOrders;

public class GetDelivererOrdersResponse
{
    public PagedResult<DelivererOrderDto> PagedOrders { get; init; } = null!;
    public int TotalDeliveries { get; init; }
    public int ActiveDeliveries { get; init; }
    public int CompletedDeliveries { get; init; }
    public decimal AverageRating { get; init; }
}