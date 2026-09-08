using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.Notifications;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Orders.Commands.AssignOrder;

public class AssignOrderHandler(
    IOrderRepository orderRepo,
    IDelivererRepository delivererRepo,
    INotificationService notificationService,
    IUnitOfWork unitOfWork) : IHandler
{
    public async Task<Result> Handle(AssignOrderCommand command, CancellationToken ct = default)
    {
        var order = await orderRepo.GetByIdAsync(command.OrderId, ct);
        if (order is null)
            return Result.Failure(OrderError.OrderNotFound(command.OrderId.ToString()));

        var deliverer = await delivererRepo.GetByIdAsync(command.DelivererId, ct);
        if (deliverer is null)
            return Result.Failure(DelivererError.NotFound(command.DelivererId));

        if (!deliverer.IsAvailable() && !deliverer.IsOnDuty())
            return Result.Failure(DelivererError.DelivererUnavailable(command.DelivererId));
        
        if ((deliverer.MinAmountOfProducts() > order.OrderItems.Sum(oi => oi.Quantity)))
            return Result.Failure(DelivererError.MinAmountOfProductsNotReached());

        deliverer.DelivererStatus = DelivererStatus.OnDuty;
        delivererRepo.Update(deliverer);

        order.DelivererId = command.DelivererId;
        order.DeliveryStatus = DeliveryStatus.Prepared;
        if (order.OrderStatus == OrderStatus.Pending)
            order.OrderStatus = OrderStatus.Confirmed;

        orderRepo.Update(order);

        await unitOfWork.SaveAsync(ct);
        
        await notificationService.SendNotificationAsync(
            deliverer.Id,
            "Dodeljena porudžbina",
            $"Dodeljena vam je porudžbina {order.OrderNumber}. Molimo vas da je preuzmete i isporučite na vreme.",
            NotificationType.Information,
            NotificationEntityType.Order,
            order.Id);
        
        return Result.Success();
    }
}
