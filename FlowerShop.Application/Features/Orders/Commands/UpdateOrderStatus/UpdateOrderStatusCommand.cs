using FlowerShop.Domain.Entities.Orders;

namespace FlowerShop.Application.Features.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand
{
    public int OrderId { get; init; }
    public string DelivererId { get; init; } = null!;
    public DeliveryStatus DeliveryStatus { get; init; }
}