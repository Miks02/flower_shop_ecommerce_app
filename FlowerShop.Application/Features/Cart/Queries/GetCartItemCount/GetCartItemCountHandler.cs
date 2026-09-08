using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Carts;

namespace FlowerShop.Application.Features.Cart.Queries.GetCartItemCount;

public class GetCartItemCountHandler(ICartRepository cartRepo) : IHandler
{
    public async Task<int> Handle(GetCartItemCountQuery request, CancellationToken ct = default)
    {
        return await cartRepo.CountItemsAsync(request.UserId, ct);
    }
}
