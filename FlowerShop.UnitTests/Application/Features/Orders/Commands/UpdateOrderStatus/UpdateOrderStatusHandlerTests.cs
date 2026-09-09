using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Orders.Commands.UpdateOrderStatus;
using FlowerShop.Domain.Entities.Notifications;
using FlowerShop.Domain.Entities.Orders;
using FluentAssertions;
using NSubstitute;

namespace FlowerShop.UnitTests.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusHandlerTests
{
    private readonly IOrderRepository _orderRepo = Substitute.For<IOrderRepository>();
    private readonly INotificationService _notificationService = Substitute.For<INotificationService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateOrderStatusHandler _sut;

    public UpdateOrderStatusHandlerTests()
    {
        _sut = new UpdateOrderStatusHandler(_orderRepo, _notificationService, _unitOfWork);
    }

    private static Order CreateOrder(DeliveryStatus deliveryStatus, int id = 1) => new()
    {
        Id = id,
        OrderNumber = "ORD-20260305-ABCDE",
        RecipientFullName = "Marko Markovic",
        RecipientPhoneNumber = "0611234567",
        OrderAddress = "Nemanjina 1",
        City = "Beograd",
        ZipCode = "11000",
        UserId = "buyer-1",
        DeliveryStatus = deliveryStatus,
        OrderStatus = OrderStatus.Confirmed
    };

    private static UpdateOrderStatusCommand CreateCommand(int orderId, DeliveryStatus target) => new()
    {
        OrderId = orderId,
        DelivererId = "deliverer-1",
        DeliveryStatus = target
    };

    [Theory]
    [InlineData(DeliveryStatus.Standby, DeliveryStatus.Prepared)]
    [InlineData(DeliveryStatus.Prepared, DeliveryStatus.InTransit)]
    [InlineData(DeliveryStatus.InTransit, DeliveryStatus.AlmostOnDestination)]
    [InlineData(DeliveryStatus.AlmostOnDestination, DeliveryStatus.Delivered)]
    public async Task Handle_WhenTransitionIsAllowed_UpdatesDeliveryStatusAndSucceeds(DeliveryStatus current, DeliveryStatus target)
    {
        var order = CreateOrder(current);
        _orderRepo.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        var command = CreateCommand(order.Id, target);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        order.DeliveryStatus.Should().Be(target);
        _orderRepo.Received(1).Update(order);
        await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(DeliveryStatus.Standby, DeliveryStatus.Delivered)]
    [InlineData(DeliveryStatus.Standby, DeliveryStatus.InTransit)]
    [InlineData(DeliveryStatus.Standby, DeliveryStatus.AlmostOnDestination)]
    [InlineData(DeliveryStatus.Prepared, DeliveryStatus.AlmostOnDestination)]
    [InlineData(DeliveryStatus.Prepared, DeliveryStatus.Delivered)]
    [InlineData(DeliveryStatus.InTransit, DeliveryStatus.Delivered)]
    [InlineData(DeliveryStatus.Delivered, DeliveryStatus.Standby)]
    public async Task Handle_WhenTransitionSkipsStepsOrGoesBackward_ReturnsInvalidDeliveryStatusTransitionError(
        DeliveryStatus current, DeliveryStatus target)
    {
        var order = CreateOrder(current);
        _orderRepo.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        var command = CreateCommand(order.Id, target);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(OrderError.InvalidDeliveryStatusTransition(current, target));
        order.DeliveryStatus.Should().Be(current);
        _orderRepo.DidNotReceive().Update(Arg.Any<Order>());
        await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDeliveryStatusBecomesDelivered_SetsOrderStatusToCompleted()
    {
        var order = CreateOrder(DeliveryStatus.AlmostOnDestination);
        _orderRepo.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        var command = CreateCommand(order.Id, DeliveryStatus.Delivered);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        order.OrderStatus.Should().Be(OrderStatus.Completed);
    }

    [Theory]
    [InlineData(DeliveryStatus.Standby, DeliveryStatus.Prepared)]
    [InlineData(DeliveryStatus.Prepared, DeliveryStatus.InTransit)]
    [InlineData(DeliveryStatus.InTransit, DeliveryStatus.AlmostOnDestination)]
    public async Task Handle_WhenTransitionSucceedsButNotDelivered_LeavesOrderStatusUnchanged(DeliveryStatus current, DeliveryStatus target)
    {
        var order = CreateOrder(current);
        _orderRepo.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        var command = CreateCommand(order.Id, target);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        order.OrderStatus.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task Handle_WhenTransitionToPrepared_SendsPreparedNotificationToBuyer()
    {
        var order = CreateOrder(DeliveryStatus.Standby);
        _orderRepo.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        var command = CreateCommand(order.Id, DeliveryStatus.Prepared);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        await _notificationService.Received(1).SendNotificationAsync(
            order.UserId,
            "Porudžbina pripremljena",
            Arg.Any<string>(),
            NotificationType.Information,
            NotificationEntityType.Order,
            order.Id);
    }

    [Fact]
    public async Task Handle_WhenTransitionToInTransit_SendsInTransitNotificationToBuyer()
    {
        var order = CreateOrder(DeliveryStatus.Prepared);
        _orderRepo.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        var command = CreateCommand(order.Id, DeliveryStatus.InTransit);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        await _notificationService.Received(1).SendNotificationAsync(
            order.UserId,
            "Porudžbina je na putu",
            Arg.Any<string>(),
            NotificationType.Information,
            NotificationEntityType.Order,
            order.Id);
    }

    [Fact]
    public async Task Handle_WhenTransitionToAlmostOnDestination_SendsAlmostOnDestinationNotificationToBuyer()
    {
        var order = CreateOrder(DeliveryStatus.InTransit);
        _orderRepo.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        var command = CreateCommand(order.Id, DeliveryStatus.AlmostOnDestination);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        await _notificationService.Received(1).SendNotificationAsync(
            order.UserId,
            "Porudžbina uskoro stiže",
            Arg.Any<string>(),
            NotificationType.Information,
            NotificationEntityType.Order,
            order.Id);
    }

    [Fact]
    public async Task Handle_WhenTransitionToDelivered_SendsDeliveredNotificationToBuyer()
    {
        var order = CreateOrder(DeliveryStatus.AlmostOnDestination);
        _orderRepo.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        var command = CreateCommand(order.Id, DeliveryStatus.Delivered);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        await _notificationService.Received(1).SendNotificationAsync(
            order.UserId,
            "Porudžbina isporučena",
            Arg.Any<string>(),
            NotificationType.Success,
            NotificationEntityType.Order,
            order.Id);
    }

    [Fact]
    public async Task Handle_WhenOrderDoesNotExist_ReturnsOrderNotFoundError()
    {
        _orderRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Order?)null);
        var command = CreateCommand(orderId: 404, DeliveryStatus.Prepared);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(OrderError.OrderNotFound("404"));
        await _notificationService.DidNotReceive().SendNotificationAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<NotificationType>(), Arg.Any<NotificationEntityType>(), Arg.Any<int?>());
    }
}
