using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.Orders;

namespace FlowerShop.Application.Features.Orders.Queries.GetAdminOrderDetails;

public record AdminOrderDetailsDto
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime OrderDate { get; init; }
    public OrderStatus OrderStatus { get; init; }
    public DeliveryStatus DeliveryStatus { get; init; }
    public string? Note { get; init; }

    public string RecipientFullName { get; init; } = null!;
    public string RecipientPhoneNumber { get; init; } = null!;
    public string OrderAddress { get; init; } = null!;
    public string City { get; init; } = null!;
    public string ZipCode { get; init; } = null!;

    public string BuyerId { get; init; } = null!;
    public string BuyerFullName { get; init; } = null!;
    public string BuyerEmail { get; init; } = null!;
    public string BuyerPhoneNumber { get; init; } = null!;

    public string? DelivererId { get; init; }
    public string? DelivererFullName { get; init; }
    public string? DelivererPhoneNumber { get; init; }
    public VehicleType? DelivererVehicleType { get; init; }

    public IReadOnlyList<AdminOrderItemDetailDto> Items { get; init; } = [];
    public IReadOnlyList<DelivererDto> AvailableDeliverers { get; init; } = [];

    public decimal Subtotal => Items.Sum(i => i.TotalPrice);
    public decimal DeliveryFee { get; init; } = 300m;
    public decimal TotalPrice => Subtotal + DeliveryFee;
    public bool IsDelivererAssigned => !string.IsNullOrEmpty(DelivererId);
}

public record AdminOrderItemDetailDto(
    int Id,
    string ProductName,
    string? ProductImagePath,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);
