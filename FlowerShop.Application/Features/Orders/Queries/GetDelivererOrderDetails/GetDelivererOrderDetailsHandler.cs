using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.LoyaltyTransactions;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Orders.Queries.GetDelivererOrderDetails;

public class GetDelivererOrderDetailsHandler(IOrderRepository orderRepo) : IHandler
{
    public async Task<Result<DelivererOrderDetailsDto>> Handle(GetDelivererOrderDetailsQuery request, CancellationToken ct = default)
    {
        var order = await orderRepo.GetByIdAsync(request.OrderId, ct);
        if (order is null)
            return Result<DelivererOrderDetailsDto>.Failure(OrderError.OrderNotFound(request.OrderId.ToString()));

        if (order.DelivererId != request.DelivererId)
            return Result<DelivererOrderDetailsDto>.Failure(new Error("OrderError_Unauthorized", "Nemate pravo da pristupite ovoj porudžbini."));

        var buyerName = $"{order.User.FirstName} {order.User.LastName}".Trim();

        var loyaltyPointsSpent = order.LoyaltyTransactions
            .Where(lt => lt.TransactionType == TransactionType.Redeemed)
            .Select(lt => lt.PreviousPoints)
            .FirstOrDefault();

        var dto = new DelivererOrderDetailsDto
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
            BuyerEmail = order.User.Email ?? string.Empty,
            BuyerPhoneNumber = order.User.PhoneNumber ?? string.Empty,
            LoyaltyPointsSpent = loyaltyPointsSpent,
            ServiceRating = order.Review?.Rating,
            ReviewComment = order.Review?.Comment,
            Items = order.OrderItems.Select(i => new DelivererOrderItemDetailDto(
                i.Id,
                i.ProductName,
                i.ProductImagePath,
                i.Quantity,
                i.UnitPrice,
                i.TotalPrice
            )).ToList(),
            DeliveryFee = 300m
        };

        return Result<DelivererOrderDetailsDto>.Success(dto);
    }
}