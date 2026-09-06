using FlowerShop.Application.Features.Orders.Queries.GetAdminOrderDetails;

namespace FlowerShop.Web.Areas.Admin.Models.Orders;

public record OrderDetailsViewModel
{
    public AdminOrderDetailsDto Order { get; init; } = null!;
    public string? SelectedDelivererId { get; init; }
}