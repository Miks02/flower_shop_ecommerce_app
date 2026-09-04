using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Carts;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Cart.Commands.AddToCart;

public class AddToCartHandler(
    ICartRepository cartRepo,
    IProductRepository productRepo,
    IUnitOfWork unitOfWork) : IHandler
{
    public async Task<Result> Handle(AddToCartCommand command, CancellationToken ct = default)
    {
        if (command.Quantity < 1)
            return Result.Failure(CartError.InvalidQuantity());

        var product = await productRepo.GetByIdAsync(command.ProductId, ct);
        if (product is null || product.IsDeleted)
            return Result.Failure(ProductError.ProductNotFound(command.ProductId));

        var cart = await cartRepo.GetByUserIdAsync(command.UserId, ct);
        if (cart is null)
        {
            cart = new Domain.Entities.Carts.Cart
            {
                UserId = command.UserId
            };
            cartRepo.Add(cart);
            await unitOfWork.SaveAsync(ct);
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == command.ProductId);
        if (existingItem is null && cart.IsLimitReached)
            return Result.Failure(CartError.CartLimitReached());

        var newQuantity = (existingItem?.Quantity ?? 0) + command.Quantity;
        if (newQuantity > product.Stock)
            return Result.Failure(CartError.InsufficientStock(product.Name, newQuantity, product.Stock));

        var unitPrice = product.PromoPrice is > 0 ? product.PromoPrice.Value : product.Price;
        
        if (existingItem is null)
        {
            cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
                Quantity = command.Quantity,
                Price = unitPrice
            });
        }
        else
        {
            existingItem.Quantity = newQuantity;
        }

        await unitOfWork.SaveAsync(ct);
        return Result.Success();
    }
}
