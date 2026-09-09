using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Common;
using FlowerShop.Domain.Entities.LoyaltyTransactions;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Orders.Queries.GetOrderReceipt;

public class GetOrderReceiptHandler(IOrderRepository orderRepo) : IHandler
{
    public async Task<Result<OrderReceiptDto>> Handle(GetOrderReceiptQuery request, CancellationToken ct = default)
    {
        var order = await orderRepo.GetByIdForUserAsync(request.OrderId, request.UserId, ct);
        if (order is null)
            return Result<OrderReceiptDto>.Failure(OrderError.OrderNotFound(request.OrderId.ToString()));

        var buyerName = $"{order.User.FirstName} {order.User.LastName}".Trim();
        var delivererName = order.Deliverer?.User != null ? $"{order.Deliverer.User.FirstName} {order.Deliverer.User.LastName}".Trim() : null;
        var delivererPhone = order.Deliverer?.User?.PhoneNumber;

        var loyaltyPointsSpent = order.LoyaltyTransactions
            .Where(lt => lt.TransactionType == TransactionType.Redeemed)
            .Select(lt => lt.PreviousPoints)
            .FirstOrDefault();
        
        var receipt = new OrderReceiptDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CreatedAt = order.CreatedAt,
            OrderDate = order.OrderDate,
            OrderStatus = order.OrderStatus,
            DeliveryStatus = order.DeliveryStatus,
            Note = order.Note,
            RecipientFullName = order.RecipientFullName,
            RecipientPhoneNumber = order.RecipientPhoneNumber,
            OrderAddress = order.OrderAddress,
            City = order.City,
            ZipCode = order.ZipCode,
            BuyerFullName = buyerName,
            BuyerEmail = order.User?.Email ?? string.Empty,
            BuyerPhoneNumber = order.User?.PhoneNumber ?? string.Empty,
            DelivererFullName = delivererName,
            DelivererPhoneNumber = delivererPhone,
            TotalPrice = order.OrderPrice + DeliveryPricing.StandardDeliveryFee,
            LoyaltyPointsSpent = loyaltyPointsSpent,
            ServiceRating = order.ServiceReview?.Rating,
            ReviewComment = order.ServiceReview?.Comment,
            Items = order.OrderItems.Select(i => new OrderReceiptItemDto(
                i.ProductName,
                i.ProductImagePath,
                i.Quantity,
                i.UnitPrice,
                i.TotalPrice
            )).ToList(),
            DeliveryFee = DeliveryPricing.StandardDeliveryFee
        };

        return Result<OrderReceiptDto>.Success(receipt);
    }
}
