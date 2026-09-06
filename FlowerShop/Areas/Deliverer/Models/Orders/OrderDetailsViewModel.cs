using FlowerShop.Application.Features.Orders.Queries.GetDelivererOrderDetails;

namespace FlowerShop.Web.Areas.Deliverer.Models.Orders;

public record OrderDetailsViewModel
{
    public DelivererOrderDetailsDto Order { get; init; } = null!;
}