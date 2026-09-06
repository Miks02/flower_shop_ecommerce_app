using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Carts;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;
using Microsoft.Extensions.Logging;

namespace FlowerShop.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandler(
    IOrderRepository orderRepo,
    ICartRepository cartRepo,
    IProductRepository productRepo,
    IUnitOfWork unitOfWork,
    ILogger<CreateOrderHandler> logger) : IHandler
{
    public async Task<Result<CreateOrderResponse>> Handle(CreateOrderCommand command, CancellationToken ct = default)
    {
        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            
            var outOfStockProducts = await productRepo.CheckStockForMultipleProductsAsync(
                command.OrderItems.Select(oi => (oi.ProductId, oi.Quantity)).ToList(),
                ct);
            
            if(outOfStockProducts.Any())
                return Result<CreateOrderResponse>.Failure(OrderError.ItemsOutOfStock(outOfStockProducts));
            
            
            var newOrder = new Order
            {
                OrderDate = command.OrderDate,
                Note = command.Note,
                RecipientFullName = command.RecipientFullName,
                RecipientPhoneNumber = command.RecipientPhoneNumber,
                ZipCode = command.ZipCode,
                City = command.City,
                OrderAddress = command.OrderAddress,
                UserId = command.BuyerId,
                OrderItems = command.OrderItems.Select(oi => new OrderItem
                {
                    ProductName = oi.ProductName,
                    ProductImagePath = oi.ProductImagePath,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
            orderRepo.Add(newOrder);

            var cart = await cartRepo.GetByUserIdAsync(command.BuyerId, ct);
            if (cart is not null)
            {
                cartRepo.Remove(cart);
            }

            await unitOfWork.SaveAsync(ct);
            await unitOfWork.CommitAsync(ct);

            return Result<CreateOrderResponse>.Success(new CreateOrderResponse(newOrder.Id, newOrder.OrderNumber));
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync(ct);
            logger.LogError(ex, "Došlo je do greške prilikom kreiranja porudžbine.");
            throw;
        }
    }
}