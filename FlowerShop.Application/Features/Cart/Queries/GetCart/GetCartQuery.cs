namespace FlowerShop.Application.Features.Cart.Queries.GetCart;

public record GetCartQuery
{
    public string UserId { get; init; } = null!;
}
