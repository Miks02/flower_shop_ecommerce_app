using FlowerShop.Domain.Entities.Orders;

namespace FlowerShop.Application.Features.Orders.Queries.GetAdminOrders;

public record GetAdminOrdersSummaryQuery
{
    public string? SearchBy { get; init; }
    public string? SortBy { get; init; } = "date_desc";
    public OrderStatus? Status { get; init; }
    public DeliveryStatus? DeliveryStatus { get; init; }
    public bool? IsAssigned { get; init; }
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
