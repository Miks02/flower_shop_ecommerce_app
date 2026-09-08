namespace FlowerShop.Application.Features.Cart.Queries.GetCartItemCount;

public record GetCartItemCountQuery
{
    public string UserId { get; init; } = null!;
}
