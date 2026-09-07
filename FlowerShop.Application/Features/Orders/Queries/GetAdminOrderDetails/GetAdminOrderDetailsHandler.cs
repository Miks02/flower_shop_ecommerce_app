using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.LoyaltyTransactions;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Orders.Queries.GetAdminOrderDetails;

public class GetAdminOrderDetailsHandler(
    IOrderRepository orderRepo,
    IDelivererRepository delivererRepo) : IHandler
{
    public async Task<Result<AdminOrderDetailsDto>> Handle(GetAdminOrderDetailsQuery request, CancellationToken ct = default)
    {
        var order = await orderRepo.GetByIdAsync(request.OrderId, ct);
        if (order is null)
            return Result<AdminOrderDetailsDto>.Failure(OrderError.OrderNotFound(request.OrderId.ToString()));

        var availableDeliverers = await delivererRepo.GetAvailableDeliverersListAsync(ct);

        var buyerName = $"{order.User.FirstName} {order.User.LastName}".Trim();
        var delivererName = order.Deliverer?.User != null ? $"{order.Deliverer.User.FirstName} {order.Deliverer.User.LastName}".Trim() : null;
        var delivererPhone = order.Deliverer?.User?.PhoneNumber;
        var vehicleType = order.Deliverer?.VehicleType;

        var loyaltyPointsSpent = order.LoyaltyTransactions
            .Where(lt => lt.TransactionType == TransactionType.Redeemed)
            .Select(lt => lt.PreviousPoints)
            .FirstOrDefault();

        var dto = new AdminOrderDetailsDto
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
            BuyerId = order.UserId,
            BuyerFullName = buyerName,
            BuyerEmail = order.User?.Email ?? string.Empty,
            BuyerPhoneNumber = order.User?.PhoneNumber ?? string.Empty,
            DelivererId = order.DelivererId,
            DelivererFullName = delivererName,
            DelivererPhoneNumber = delivererPhone,
            DelivererVehicleType = vehicleType,
            LoyaltyPointsSpent = loyaltyPointsSpent,
            Items = order.OrderItems.Select(i => new AdminOrderItemDetailDto(
                i.Id,
                i.ProductName,
                i.ProductImagePath,
                i.Quantity,
                i.UnitPrice,
                i.TotalPrice
            )).ToList(),
            AvailableDeliverers = availableDeliverers,
            DeliveryFee = 300m
        };

        return Result<AdminOrderDetailsDto>.Success(dto);
    }
}
