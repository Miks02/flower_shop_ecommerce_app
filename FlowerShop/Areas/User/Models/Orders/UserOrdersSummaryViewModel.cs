using FlowerShop.Application.Features.Orders.Queries.GetUserOrders;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Web.Areas.User.Models.Orders;

public record UserOrdersSummaryViewModel
{
    public PagedResult<UserOrderDto> PagedOrders { get; init; } = null!;
    public int TotalOrders { get; init; }
    public int PendingOrders { get; init; }
    public int InDeliveryOrders { get; init; }
    public int CompletedOrders { get; init; }
    public string? SearchBy { get; init; }
    public string? SortBy { get; init; } = "date_desc";
    public OrderStatus? SelectedStatus { get; init; }
}
