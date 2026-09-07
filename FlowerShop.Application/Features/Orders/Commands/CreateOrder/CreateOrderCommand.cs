namespace FlowerShop.Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand
{
    public string BuyerId { get; init; } = null!;
    public string RecipientFullName { get; init; } = null!;
    public string RecipientPhoneNumber { get; init; } = null!;

    public string OrderAddress { get; init; } = null!;
    public string City { get; init; } = null!;
    public string ZipCode { get; init; } = null!;
    
    public string? Note { get; init; }
    public DateTime OrderDate { get; init; }
    public bool UseLoyaltyPoints { get; init; }
    
    public IReadOnlyList<OrderItemDto> OrderItems { get; init; } = [];

    public record OrderItemDto
    {
        public int ProductId { get; init; }
        public string ProductName { get; init; } = null!;
        public string? ProductImagePath { get; init; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
    }
};