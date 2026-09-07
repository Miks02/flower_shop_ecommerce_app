using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Notifications;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusHandler(
    IOrderRepository orderRepo,
    INotificationService notificationService,
    IUnitOfWork unitOfWork) : IHandler
{
    public async Task<Result> Handle(UpdateOrderStatusCommand command, CancellationToken ct = default)
    {
        var order = await orderRepo.GetByIdAsync(command.OrderId, ct);
        if (order is null)
            return Result.Failure(OrderError.OrderNotFound(command.OrderId.ToString()));

        if (!IsValidTransition(order.DeliveryStatus, command.DeliveryStatus))
            return Result.Failure(OrderError.InvalidDeliveryStatusTransition(order.DeliveryStatus, command.DeliveryStatus));

        order.DeliveryStatus = command.DeliveryStatus;

        if (command.DeliveryStatus == DeliveryStatus.Delivered)
        {
            order.OrderStatus = OrderStatus.Completed;
        }

        orderRepo.Update(order);
        await unitOfWork.SaveAsync(ct);

        var (title, message, type) = GetNotificationContent(command.DeliveryStatus, order.OrderNumber);
        if (title is not null && message is not null)
        {
            await notificationService.SendNotificationAsync(
                order.UserId,
                title,
                message,
                type,
                NotificationEntityType.Order,
                order.Id);
        }

        return Result.Success();
    }

    private static (string? Title, string? Message, NotificationType type) GetNotificationContent(DeliveryStatus status, string orderNumber)
    {
        return status switch
        {
            DeliveryStatus.Prepared => (
                "Porudžbina pripremljena",
                $"Vaša porudžbina {orderNumber} je pripremljena i uskoro kreće ka vama.",
                NotificationType.Information),

            DeliveryStatus.OnTheWay or DeliveryStatus.InTransit => (
                "Porudžbina je na putu",
                $"Vaša porudžbina {orderNumber} je na putu ka odredištu. Očekujte isporuku uskoro.",
                NotificationType.Information),

            DeliveryStatus.Delivered => (
                "Porudžbina isporučena",
                $"Vaša porudžbina {orderNumber} je uspešno isporučena. Hvala vam na poverenju!",
                NotificationType.Success),

            _ => (null, null, NotificationType.Information)
        };
    }

    private static bool IsValidTransition(DeliveryStatus current, DeliveryStatus target)
    {
        return (current, target) switch
        {
            (DeliveryStatus.Standby, DeliveryStatus.Prepared) => true,
            (DeliveryStatus.Prepared, DeliveryStatus.OnTheWay) => true,
            (DeliveryStatus.Prepared, DeliveryStatus.InTransit) => true,
            (DeliveryStatus.OnTheWay, DeliveryStatus.Delivered) => true,
            (DeliveryStatus.InTransit, DeliveryStatus.Delivered) => true,
            _ => false
        };
    }
}