namespace FlowerShop.Application.Features.Cart.Commands.RemoveCartItem;

public record RemoveCartItemCommand
{
    public string UserId { get; init; } = null!;
    public int CartItemId { get; init; }
}
