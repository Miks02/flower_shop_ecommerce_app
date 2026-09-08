using FlowerShop.Domain.Entities.Orders;

namespace FlowerShop.Application.Features.Orders.Queries.GetAdminOrders;

public record AdminOrderDto
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = null!;
    public string RecipientFullName { get; init; } = null!;
    public string RecipientPhoneNumber { get; init; } = null!;
    public string OrderAddress { get; init; } = null!;
    public string City { get; init; } = null!;
    public string ZipCode { get; init; } = null!;
    public OrderStatus OrderStatus { get; init; }
    public DeliveryStatus DeliveryStatus { get; init; }
    public DateTime OrderDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? Note { get; init; }
    public decimal OrderPrice { get; init; }
    public int TotalItemsCount { get; init; }
    public bool IsRated { get; init; }
    public string? BuyerFullName { get; init; }
    public string? BuyerEmail { get; init; }
    public string? DelivererId { get; init; }
    public string? DelivererFullName { get; init; }
    public IReadOnlyList<AdminOrderItemPreviewDto> Items { get; init; } = [];
}

public record AdminOrderItemPreviewDto
{
    public int Id { get; init; }
    public string ProductName { get; init; } = null!;
    public string? ProductImagePath { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice => Quantity * UnitPrice;
}
