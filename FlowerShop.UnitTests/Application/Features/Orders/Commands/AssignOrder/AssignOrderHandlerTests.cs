using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Orders.Commands.AssignOrder;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.Orders;
using FluentAssertions;
using NSubstitute;

namespace FlowerShop.UnitTests.Application.Features.Orders.Commands.AssignOrder;

public class AssignOrderHandlerTests
{
    private readonly IOrderRepository _orderRepo = Substitute.For<IOrderRepository>();
    private readonly IDelivererRepository _delivererRepo = Substitute.For<IDelivererRepository>();
    private readonly INotificationService _notificationService = Substitute.For<INotificationService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly AssignOrderHandler _sut;

    public AssignOrderHandlerTests()
    {
        _sut = new AssignOrderHandler(_orderRepo, _delivererRepo, _notificationService, _unitOfWork);
    }

    private static Order CreateOrder(int quantity, int id = 1, OrderStatus orderStatus = OrderStatus.Pending) => new()
    {
        Id = id,
        OrderNumber = "ORD-20260305-ABCDE",
        RecipientFullName = "Marko Markovic",
        RecipientPhoneNumber = "0611234567",
        OrderAddress = "Nemanjina 1",
        City = "Beograd",
        ZipCode = "11000",
        UserId = "buyer-1",
        OrderStatus = orderStatus,
        OrderItems = quantity > 0
            ? [new OrderItem { ProductName = "Ruza", Quantity = quantity, UnitPrice = 100m }]
            : []
    };

    private static Deliverer CreateDeliverer(
        VehicleType vehicleType,
        DelivererStatus status = DelivererStatus.Available,
        string id = "deliverer-1") => new()
    {
        Id = id,
        VehicleType = vehicleType,
        DelivererStatus = status
    };

    private static AssignOrderCommand CreateCommand(int orderId, string delivererId) => new()
    {
        OrderId = orderId,
        DelivererId = delivererId
    };

    [Fact]
    public async Task Handle_WhenDelivererIsUnavailable_ReturnsDelivererUnavailableError()
    {
        var order = CreateOrder(quantity: 5);
        var deliverer = CreateDeliverer(VehicleType.Car, DelivererStatus.Unavailable);
        _orderRepo.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        _delivererRepo.GetByIdAsync(deliverer.Id, Arg.Any<CancellationToken>()).Returns(deliverer);
        var command = CreateCommand(order.Id, deliverer.Id);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(DelivererError.DelivererUnavailable(deliverer.Id));
        _delivererRepo.DidNotReceive().Update(Arg.Any<Deliverer>());
        _orderRepo.DidNotReceive().Update(Arg.Any<Order>());
    }

    [Theory]
    [InlineData(VehicleType.Bicycle, 0)]
    [InlineData(VehicleType.Scooter, 2)]
    [InlineData(VehicleType.Car, 4)]
    public async Task Handle_WhenItemCountIsBelowVehicleMinimum_ReturnsMinAmountOfProductsNotReachedError(
        VehicleType vehicleType, int quantity)
    {
        var order = CreateOrder(quantity);
        var deliverer = CreateDeliverer(vehicleType);
        _orderRepo.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        _delivererRepo.GetByIdAsync(deliverer.Id, Arg.Any<CancellationToken>()).Returns(deliverer);
        var command = CreateCommand(order.Id, deliverer.Id);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(DelivererError.MinAmountOfProductsNotReached());
        _delivererRepo.DidNotReceive().Update(Arg.Any<Deliverer>());
        _orderRepo.DidNotReceive().Update(Arg.Any<Order>());
    }

    [Theory]
    [InlineData(VehicleType.Bicycle, 1)]
    [InlineData(VehicleType.Scooter, 3)]
    [InlineData(VehicleType.Car, 5)]
    public async Task Handle_WhenItemCountExactlyMeetsVehicleMinimum_AssignsOrderSuccessfully(
        VehicleType vehicleType, int quantity)
    {
        var order = CreateOrder(quantity);
        var deliverer = CreateDeliverer(vehicleType);
        _orderRepo.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        _delivererRepo.GetByIdAsync(deliverer.Id, Arg.Any<CancellationToken>()).Returns(deliverer);
        var command = CreateCommand(order.Id, deliverer.Id);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenAssignmentSucceeds_SetsDelivererToOnDutyDeliveryToPreparedAndOrderToConfirmed()
    {
        var order = CreateOrder(quantity: 5, orderStatus: OrderStatus.Pending);
        var deliverer = CreateDeliverer(VehicleType.Car, DelivererStatus.Available);
        _orderRepo.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        _delivererRepo.GetByIdAsync(deliverer.Id, Arg.Any<CancellationToken>()).Returns(deliverer);
        var command = CreateCommand(order.Id, deliverer.Id);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        deliverer.DelivererStatus.Should().Be(DelivererStatus.OnDuty);
        order.DeliveryStatus.Should().Be(DeliveryStatus.Prepared);
        order.OrderStatus.Should().Be(OrderStatus.Confirmed);
        order.DelivererId.Should().Be(deliverer.Id);
        _delivererRepo.Received(1).Update(deliverer);
        _orderRepo.Received(1).Update(order);
        await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDelivererIsAlreadyOnDuty_AllowsAssignment()
    {
        var order = CreateOrder(quantity: 5);
        var deliverer = CreateDeliverer(VehicleType.Car, DelivererStatus.OnDuty);
        _orderRepo.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        _delivererRepo.GetByIdAsync(deliverer.Id, Arg.Any<CancellationToken>()).Returns(deliverer);
        var command = CreateCommand(order.Id, deliverer.Id);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        deliverer.DelivererStatus.Should().Be(DelivererStatus.OnDuty);
    }

    [Fact]
    public async Task Handle_WhenOrderIsNotPending_DoesNotChangeOrderStatus()
    {
        var order = CreateOrder(quantity: 5, orderStatus: OrderStatus.Confirmed);
        var deliverer = CreateDeliverer(VehicleType.Car);
        _orderRepo.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        _delivererRepo.GetByIdAsync(deliverer.Id, Arg.Any<CancellationToken>()).Returns(deliverer);
        var command = CreateCommand(order.Id, deliverer.Id);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        order.OrderStatus.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task Handle_WhenOrderDoesNotExist_ReturnsOrderNotFoundError()
    {
        _orderRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Order?)null);
        var command = CreateCommand(orderId: 404, "deliverer-1");

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(OrderError.OrderNotFound("404"));
    }

    [Fact]
    public async Task Handle_WhenDelivererDoesNotExist_ReturnsDelivererNotFoundError()
    {
        var order = CreateOrder(quantity: 5);
        _orderRepo.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        _delivererRepo.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Deliverer?)null);
        var command = CreateCommand(order.Id, "unknown-deliverer");

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(DelivererError.NotFound("unknown-deliverer"));
    }
}
