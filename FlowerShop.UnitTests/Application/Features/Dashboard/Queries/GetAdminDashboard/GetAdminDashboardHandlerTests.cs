using FlowerShop.Application.Features.Dashboard.Queries.GetAdminDashboard;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Domain.Entities.Products;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace FlowerShop.UnitTests.Application.Features.Dashboard.Queries.GetAdminDashboard;

public class GetAdminDashboardHandlerTests
{
    private readonly IOrderRepository _orderRepo = Substitute.For<IOrderRepository>();
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly IDelivererRepository _delivererRepo = Substitute.For<IDelivererRepository>();
    private readonly UserManager<User> _userManager = Substitute.For<UserManager<User>>(
        Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);
    private readonly GetAdminDashboardHandler _sut;

    public GetAdminDashboardHandlerTests()
    {
        _sut = new GetAdminDashboardHandler(_orderRepo, _productRepo, _delivererRepo, _userManager);

        _orderRepo.GetAdminOrderStatsAsync(Arg.Any<CancellationToken>())
            .Returns((TotalOrders: 0, PendingOrders: 0, UnassignedOrders: 0, InDeliveryOrders: 0, CompletedOrders: 0, ActiveOrders: 0));
        _orderRepo.GetTodaySalesStatsAsync(Arg.Any<CancellationToken>())
            .Returns((TotalSales: 0m, NewOrdersCount: 0));
        _orderRepo.GetRecentOrdersAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<Order>)[]);
        _productRepo.CountAvailableProductsAsync(Arg.Any<CancellationToken>()).Returns(0);
        _delivererRepo.GetStatisticsAsync(Arg.Any<CancellationToken>())
            .Returns(new DelivererStatisticsDto(0, 0, 0, 0, 0, 0, 0, 0m));
        _userManager.GetUsersInRoleAsync(Arg.Any<string>()).Returns((IList<User>)[]);
    }

    private static Order CreateOrder(int id, User user, decimal price, OrderStatus status, DateTime createdAt) => new()
    {
        Id = id,
        OrderNumber = $"ORD-{id}",
        RecipientFullName = "Marko Markovic",
        RecipientPhoneNumber = "0611234567",
        OrderAddress = "Nemanjina 1",
        City = "Beograd",
        ZipCode = "11000",
        UserId = user.Id,
        User = user,
        OrderPrice = price,
        OrderStatus = status,
        CreatedAt = createdAt
    };

    private static User CreateUser(string firstName, string lastName, string id = "user-1") => new()
    {
        Id = id,
        FirstName = firstName,
        LastName = lastName
    };

    [Fact]
    public async Task Handle_AggregatesStatisticsFromAllRepositories()
    {
        _orderRepo.GetAdminOrderStatsAsync(Arg.Any<CancellationToken>())
            .Returns((TotalOrders: 40, PendingOrders: 5, UnassignedOrders: 2, InDeliveryOrders: 8, CompletedOrders: 25, ActiveOrders: 13));
        _orderRepo.GetTodaySalesStatsAsync(Arg.Any<CancellationToken>())
            .Returns((TotalSales: 4500.50m, NewOrdersCount: 7));
        _productRepo.CountAvailableProductsAsync(Arg.Any<CancellationToken>()).Returns(120);
        _delivererRepo.GetStatisticsAsync(Arg.Any<CancellationToken>())
            .Returns(new DelivererStatisticsDto(10, 4, 3, 3, 5, 3, 2, 4.5m));
        _userManager.GetUsersInRoleAsync(Arg.Any<string>())
            .Returns((IList<User>)[CreateUser("Ana", "Anic"), CreateUser("Petar", "Petrovic", "user-2")]);

        var response = await _sut.Handle();

        response.TodaySales.Should().Be(4500.50m);
        response.TodayNewOrders.Should().Be(7);
        response.ActiveOrders.Should().Be(13);
        response.AvailableProducts.Should().Be(120);
        response.DeliverersOnDuty.Should().Be(3);
        response.RegisteredCustomers.Should().Be(2);
    }

    [Fact]
    public async Task Handle_MapsRecentOrdersToAdminRecentOrderDto()
    {
        var user = CreateUser("Ana", "Anic");
        var createdAt = new DateTime(2026, 3, 1, 10, 0, 0);
        var order = CreateOrder(1, user, price: 2500m, OrderStatus.Confirmed, createdAt);

        _orderRepo.GetRecentOrdersAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<Order>)[order]);

        var response = await _sut.Handle();

        response.RecentOrders.Should().ContainSingle().Which.Should().BeEquivalentTo(new
        {
            Id = 1,
            OrderNumber = order.OrderNumber,
            CustomerFullName = "Ana Anic",
            OrderPrice = 2500m,
            OrderStatus = OrderStatus.Confirmed,
            CreatedAt = createdAt
        });
    }

    [Fact]
    public async Task Handle_WhenCustomerLastNameIsEmpty_TrimsTrailingWhitespaceFromFullName()
    {
        var user = CreateUser("Ana", "");
        var order = CreateOrder(1, user, price: 100m, OrderStatus.Pending, DateTime.UtcNow);

        _orderRepo.GetRecentOrdersAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<Order>)[order]);

        var response = await _sut.Handle();

        response.RecentOrders.Single().CustomerFullName.Should().Be("Ana");
    }

    [Fact]
    public async Task Handle_WhenNoRecentOrders_ReturnsEmptyRecentOrdersList()
    {
        _orderRepo.GetRecentOrdersAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<Order>)[]);

        var response = await _sut.Handle();

        response.RecentOrders.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_RequestsFiveRecentOrders()
    {
        await _sut.Handle();

        await _orderRepo.Received(1).GetRecentOrdersAsync(5, Arg.Any<CancellationToken>());
    }
}
