using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Carts;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Cart.Commands.RemoveCartItem;

public class RemoveCartItemHandler(
    ICartRepository cartRepo,
    IUnitOfWork unitOfWork) : IHandler
{
    public async Task<Result> Handle(RemoveCartItemCommand command, CancellationToken ct = default)
    {
        var item = await cartRepo.GetItemByIdForUserAsync(command.CartItemId, command.UserId, ct);
        if (item is null)
            return Result.Failure(CartError.CartItemNotFound());

        cartRepo.RemoveItem(item);
        await unitOfWork.SaveAsync(ct);

        return Result.Success();
    }
}
