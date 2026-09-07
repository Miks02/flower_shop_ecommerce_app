using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Carts;
using FlowerShop.Domain.Entities.LoyaltyTransactions;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandler(
    IOrderRepository orderRepo,
    ICartRepository cartRepo,
    IProductRepository productRepo,
    ILoyaltyTransactionRepository loyaltyRepo,
    IUnitOfWork unitOfWork) : IHandler
{
    private const int MinPointsToRedeem = 1000;
    private const int PointsEarnedPerOrder = 100;

    public async Task<Result<CreateOrderResponse>> Handle(CreateOrderCommand command, CancellationToken ct = default)
    {
          var outOfStockProducts = await productRepo.CheckStockForMultipleProductsAsync(
                command.OrderItems.Select(oi => (oi.ProductId, oi.Quantity)).ToList(),
                ct);

            if (outOfStockProducts.Any())
                return Result<CreateOrderResponse>.Failure(OrderError.ItemsOutOfStock(outOfStockProducts));

            var currentPoints = await loyaltyRepo.GetCurrentLoyaltyPoints(command.BuyerId, ct);

            if (command.UseLoyaltyPoints && currentPoints < MinPointsToRedeem)
                return Result<CreateOrderResponse>.Failure(LoyaltyTransactionErrors.InsufficientPoints());

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
                OrderPrice = command.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
                OrderItems = command.OrderItems.Select(oi => new OrderItem
                {
                    ProductName = oi.ProductName,
                    ProductImagePath = oi.ProductImagePath,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };

            var pointsBalanceAfterRedeem = currentPoints;

            if (command.UseLoyaltyPoints)
            {
                newOrder.OrderPrice = currentPoints > newOrder.OrderPrice
                    ? 0
                    : newOrder.OrderPrice - currentPoints;

                pointsBalanceAfterRedeem = 0;
                
                orderRepo.Add(newOrder);
                loyaltyRepo.Add(new LoyaltyTransaction
                {
                    TransactionType = TransactionType.Redeemed,
                    UserId = command.BuyerId,
                    Order = newOrder, 
                    CurrentPoints = pointsBalanceAfterRedeem,
                    PreviousPoints = currentPoints
                });
            }
            else 
                orderRepo.Add(newOrder);

            loyaltyRepo.Add(new LoyaltyTransaction
            {
                TransactionType = TransactionType.Earned,
                UserId = command.BuyerId,
                Order = newOrder, 
                CurrentPoints = pointsBalanceAfterRedeem + PointsEarnedPerOrder,
                PreviousPoints = currentPoints
            });

            var cart = await cartRepo.GetByUserIdAsync(command.BuyerId, ct);
            if (cart is not null)
                cartRepo.Remove(cart);

            await unitOfWork.SaveAsync(ct); 
        
            return Result<CreateOrderResponse>.Success(new CreateOrderResponse(newOrder.Id, newOrder.OrderNumber));
    }   
}