using FlowerShop.Application.Features.Orders.Queries.GetAdminOrders;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Web.Areas.Admin.Models.Orders;

public record OrderSummaryViewModel
{
    public PagedResult<AdminOrderDto> PagedOrders { get; init; } = null!;
    public int TotalOrders { get; init; }
    public int PendingOrders { get; init; }
    public int UnassignedOrders { get; init; }
    public int InDeliveryOrders { get; init; }
    public int CompletedOrders { get; init; }
    public string? SearchBy { get; init; }
    public string? SortBy { get; init; } = "date_desc";
    public OrderStatus? SelectedStatus { get; init; }
    public DeliveryStatus? SelectedDeliveryStatus { get; init; }
    public bool? IsAssigned { get; init; }
}