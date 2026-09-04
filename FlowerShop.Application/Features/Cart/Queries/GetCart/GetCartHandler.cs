using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Carts;

namespace FlowerShop.Application.Features.Cart.Queries.GetCart;

public class GetCartHandler(ICartRepository cartRepo) : IHandler
{
    public async Task<GetCartResponse> Handle(GetCartQuery request, CancellationToken ct = default)
    {
        var cart = await cartRepo.GetByUserIdAsync(request.UserId, ct);
        if (cart is null)
            return new GetCartResponse();

        return new GetCartResponse
        {
            Id = cart.Id,
            Items = cart.Items
                .OrderBy(i => i.Id)
                .Select(i => new CartItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity = i.Quantity,
                    Price = i.Price
                })
                .ToList()
        };
    }
}
