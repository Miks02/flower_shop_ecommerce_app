using FlowerShop.Domain.Entities.Orders;

namespace FlowerShop.Application.Features.Orders.Queries.GetUserOrders;

public record GetUserOrdersSummaryQuery
{
    public string UserId { get; init; } = null!;
    public string? SearchBy { get; init; }
    public string? SortBy { get; init; } = "date_desc";
    public OrderStatus? Status { get; init; }
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 8;
}
