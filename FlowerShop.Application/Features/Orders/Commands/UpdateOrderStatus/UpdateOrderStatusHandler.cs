using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusHandler(
    IOrderRepository orderRepo,
    IUnitOfWork unitOfWork) : IHandler
{
    public async Task<Result> Handle(UpdateOrderStatusCommand command, CancellationToken ct = default)
    {
        var order = await orderRepo.GetByIdAsync(command.OrderId, ct);
        if (order is null)
            return Result.Failure(OrderError.OrderNotFound(command.OrderId.ToString()));

        if (order.DelivererId != command.DelivererId)
            return Result.Failure(new Error("OrderError_Unauthorized", "Nemate pravo da ažurirate ovu porudžbinu."));

        if (!IsValidTransition(order.DeliveryStatus, command.DeliveryStatus))
            return Result.Failure(OrderError.InvalidDeliveryStatusTransition(order.DeliveryStatus, command.DeliveryStatus));

        order.DeliveryStatus = command.DeliveryStatus;

        if (command.DeliveryStatus == DeliveryStatus.Delivered)
        {
            order.OrderStatus = OrderStatus.Completed;
        }

        orderRepo.Update(order);
        await unitOfWork.SaveAsync(ct);

        return Result.Success();
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