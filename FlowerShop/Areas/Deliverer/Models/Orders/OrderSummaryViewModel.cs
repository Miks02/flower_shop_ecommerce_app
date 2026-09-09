using FlowerShop.Application.Features.Orders.Queries.GetDelivererOrders;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Web.Areas.Deliverer.Models.Orders;

public record OrderSummaryViewModel
{
    public PagedResult<DelivererOrderDto> PagedOrders { get; init; } = null!;
    public int TotalDeliveries { get; init; }
    public int ActiveDeliveries { get; init; }
    public int CompletedDeliveries { get; init; }
    public decimal AverageRating { get; init; }
    public string? SearchBy { get; init; }
    public string? SortBy { get; init; } = "date_desc";
    public OrderStatus? SelectedStatus { get; init; }
    public DeliveryStatus? SelectedDeliveryStatus { get; init; }
}