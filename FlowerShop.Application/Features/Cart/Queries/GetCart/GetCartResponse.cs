namespace FlowerShop.Application.Features.Cart.Queries.GetCart;

public record GetCartResponse
{
    public int? Id { get; init; }
    public IReadOnlyList<CartItemDto> Items { get; init; } = [];
    public int ItemCount => Items.Count;
    public decimal Total => Items.Sum(i => i.LineTotal);
}

public record CartItemDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal Price { get; init; }
    public decimal LineTotal => Price * Quantity;
}
