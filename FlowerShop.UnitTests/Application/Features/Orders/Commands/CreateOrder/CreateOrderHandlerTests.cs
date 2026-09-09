using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Orders.Commands.CreateOrder;
using FlowerShop.Domain.Entities.Carts;
using FlowerShop.Domain.Entities.LoyaltyTransactions;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Domain.Entities.Products;
using FluentAssertions;
using NSubstitute;

namespace FlowerShop.UnitTests.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandlerTests
{
    private readonly IOrderRepository _orderRepo = Substitute.For<IOrderRepository>();
    private readonly ICartRepository _cartRepo = Substitute.For<ICartRepository>();
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly ILoyaltyTransactionRepository _loyaltyRepo = Substitute.For<ILoyaltyTransactionRepository>();
    private readonly INotificationService _notificationService = Substitute.For<INotificationService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateOrderHandler _sut;

    public CreateOrderHandlerTests()
    {
        _sut = new CreateOrderHandler(_orderRepo, _cartRepo, _productRepo, _loyaltyRepo, _notificationService, _unitOfWork);
    }

    private static Product CreateProduct(int id, string name, int stock) => new()
    {
        Id = id,
        Name = name,
        ImageUrl = "image.png",
        Price = 10,
        Stock = stock,
        CreatedBy = "admin",
        CategoryId = 1
    };

    private static CreateOrderCommand.OrderItemDto CreateItemDto(int productId, string name, int quantity, decimal unitPrice) => new()
    {
        ProductId = productId,
        ProductName = name,
        ProductImagePath = null,
        Quantity = quantity,
        UnitPrice = unitPrice
    };

    private static CreateOrderCommand CreateCommand(
        IReadOnlyList<CreateOrderCommand.OrderItemDto> items,
        bool useLoyaltyPoints = false,
        string buyerId = "buyer-1") => new()
    {
        BuyerId = buyerId,
        RecipientFullName = "Marko Markovic",
        RecipientPhoneNumber = "0611234567",
        OrderAddress = "Nemanjina 1",
        City = "Beograd",
        ZipCode = "11000",
        OrderDate = new DateTime(2026, 3, 5),
        UseLoyaltyPoints = useLoyaltyPoints,
        OrderItems = items
    };

    [Fact]
    public async Task Handle_WhenStockIsSufficient_CreatesOrderSuccessfully()
    {
        var product = CreateProduct(1, "Ruza", stock: 10);
        var command = CreateCommand([CreateItemDto(1, "Ruza", quantity: 2, unitPrice: 100m)]);

        _productRepo.GetProductsByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([product]);
        _loyaltyRepo.GetCurrentLoyaltyPoints(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(0);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        result.Payload.Should().NotBeNull();
        result.Payload!.OrderId.Should().Be(0);
        result.Payload.OrderNumber.Should().NotBeNullOrWhiteSpace();
        _orderRepo.Received(1).Add(Arg.Any<Order>());
        await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsProductsNotFoundError()
    {
        var command = CreateCommand([CreateItemDto(99, "Nepostojeci proizvod", quantity: 1, unitPrice: 50m)]);

        _productRepo.GetProductsByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([]);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(ProductError.ProductsNotFound([99]));
        _orderRepo.DidNotReceive().Add(Arg.Any<Order>());
    }

    [Fact]
    public async Task Handle_WhenStockIsInsufficient_ReturnsItemsOutOfStockError()
    {
        var product = CreateProduct(1, "Ruza", stock: 1);
        var command = CreateCommand([CreateItemDto(1, "Ruza", quantity: 5, unitPrice: 50m)]);

        _productRepo.GetProductsByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([product]);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(OrderError.ItemsOutOfStock(["Ruza"]));
        _orderRepo.DidNotReceive().Add(Arg.Any<Order>());
    }

    [Fact]
    public async Task Handle_WhenLoyaltyPointsAboveThreshold_ReducesOrderPriceResetsPointsAndRecordsRedeemedTransaction()
    {
        var product = CreateProduct(1, "Ruza", stock: 10);
        var command = CreateCommand(
            [CreateItemDto(1, "Ruza", quantity: 10, unitPrice: 200m)],
            useLoyaltyPoints: true);

        _productRepo.GetProductsByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([product]);
        _loyaltyRepo.GetCurrentLoyaltyPoints(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(1000);

        Order? capturedOrder = null;
        _orderRepo.When(x => x.Add(Arg.Any<Order>())).Do(x => capturedOrder = x.Arg<Order>());

        var capturedTransactions = new List<LoyaltyTransaction>();
        _loyaltyRepo.When(x => x.Add(Arg.Any<LoyaltyTransaction>())).Do(x => capturedTransactions.Add(x.Arg<LoyaltyTransaction>()));

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        capturedOrder.Should().NotBeNull();
        capturedOrder!.OrderPrice.Should().Be(1000m); // 2000 (order total) - 1000 (points redeemed)

        var redeemed = capturedTransactions.Should().ContainSingle(t => t.TransactionType == TransactionType.Redeemed).Subject;
        redeemed.PreviousPoints.Should().Be(1000);
        redeemed.CurrentPoints.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenLoyaltyPointsBelowThreshold_ReturnsInsufficientPointsError()
    {
        var product = CreateProduct(1, "Ruza", stock: 10);
        var command = CreateCommand(
            [CreateItemDto(1, "Ruza", quantity: 1, unitPrice: 50m)],
            useLoyaltyPoints: true);

        _productRepo.GetProductsByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([product]);
        _loyaltyRepo.GetCurrentLoyaltyPoints(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(999);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be(LoyaltyTransactionErrors.InsufficientPoints());
        _orderRepo.DidNotReceive().Add(Arg.Any<Order>());
    }

    [Fact]
    public async Task Handle_Always_RecordsEarnedTransactionOfHundredPoints()
    {
        var product = CreateProduct(1, "Ruza", stock: 10);
        var command = CreateCommand([CreateItemDto(1, "Ruza", quantity: 1, unitPrice: 50m)]);

        _productRepo.GetProductsByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([product]);
        _loyaltyRepo.GetCurrentLoyaltyPoints(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(200);

        var capturedTransactions = new List<LoyaltyTransaction>();
        _loyaltyRepo.When(x => x.Add(Arg.Any<LoyaltyTransaction>())).Do(x => capturedTransactions.Add(x.Arg<LoyaltyTransaction>()));

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        var earned = capturedTransactions.Should().ContainSingle(t => t.TransactionType == TransactionType.Earned).Subject;
        earned.PreviousPoints.Should().Be(200);
        earned.CurrentPoints.Should().Be(300); // 200 (not redeemed) + 100 earned
    }

    [Fact]
    public async Task Handle_WhenOrderCreatedSuccessfully_RemovesCart()
    {
        var product = CreateProduct(1, "Ruza", stock: 10);
        var command = CreateCommand([CreateItemDto(1, "Ruza", quantity: 1, unitPrice: 50m)]);
        var cart = new Cart { UserId = command.BuyerId };

        _productRepo.GetProductsByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([product]);
        _loyaltyRepo.GetCurrentLoyaltyPoints(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(0);
        _cartRepo.GetByUserIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(cart);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        _cartRepo.Received(1).Remove(cart);
    }

    [Fact]
    public async Task Handle_WhenOrderCreatedSuccessfully_DecreasesProductStockByOrderedQuantity()
    {
        var product = CreateProduct(1, "Ruza", stock: 10);
        var command = CreateCommand([CreateItemDto(1, "Ruza", quantity: 3, unitPrice: 50m)]);

        _productRepo.GetProductsByIdsAsync(Arg.Any<IReadOnlyList<int>>(), Arg.Any<CancellationToken>())
            .Returns([product]);
        _loyaltyRepo.GetCurrentLoyaltyPoints(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(0);

        var result = await _sut.Handle(command);

        result.IsSuccess.Should().BeTrue();
        _productRepo.Received(1).Update(Arg.Is<Product>(p => p.Id == 1 && p.Stock == 7));
    }
}
