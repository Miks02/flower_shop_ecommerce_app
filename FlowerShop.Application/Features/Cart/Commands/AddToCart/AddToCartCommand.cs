namespace FlowerShop.Application.Features.Cart.Commands.AddToCart;

public record AddToCartCommand
{
    public string UserId { get; init; } = null!;
    public int ProductId { get; init; }
    public int Quantity { get; init; } = 1;
}
