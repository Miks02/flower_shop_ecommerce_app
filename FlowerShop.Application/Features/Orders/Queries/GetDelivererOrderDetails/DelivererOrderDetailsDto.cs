using FlowerShop.Domain.Common;
using FlowerShop.Domain.Entities.Orders;

namespace FlowerShop.Application.Features.Orders.Queries.GetDelivererOrderDetails;

public record DelivererOrderDetailsDto
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

    public string BuyerFullName { get; init; } = null!;
    public string BuyerEmail { get; init; } = null!;
    public string BuyerPhoneNumber { get; init; } = null!;

    public decimal? ServiceRating { get; init; }
    public string? ReviewComment { get; init; }

    public IReadOnlyList<DelivererOrderItemDetailDto> Items { get; init; } = [];

    public decimal Subtotal => Items.Sum(i => i.TotalPrice);
    public decimal DeliveryFee { get; init; } = DeliveryPricing.StandardDeliveryFee;
    public int LoyaltyPointsSpent { get; init; }
    public decimal TotalPrice => Subtotal + DeliveryFee - LoyaltyPointsSpent;
}

public record DelivererOrderItemDetailDto(
    int Id,
    string ProductName,
    string? ProductImagePath,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);