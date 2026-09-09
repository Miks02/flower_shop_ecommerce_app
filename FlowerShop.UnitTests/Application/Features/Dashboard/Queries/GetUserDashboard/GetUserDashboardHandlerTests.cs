using FlowerShop.Application.Features.Dashboard.Queries.GetUserDashboard;
using FlowerShop.Domain.Entities.LoyaltyTransactions;
using FlowerShop.Domain.Entities.Orders;
using FluentAssertions;
using NSubstitute;

namespace FlowerShop.UnitTests.Application.Features.Dashboard.Queries.GetUserDashboard;

public class GetUserDashboardHandlerTests
{
    private const string UserId = "buyer-1";

    private readonly IOrderRepository _orderRepo = Substitute.For<IOrderRepository>();
    private readonly ILoyaltyTransactionRepository _loyaltyRepo = Substitute.For<ILoyaltyTransactionRepository>();
    private readonly GetUserDashboardHandler _sut;

    public GetUserDashboardHandlerTests()
    {
        _sut = new GetUserDashboardHandler(_orderRepo, _loyaltyRepo);

        _orderRepo.GetUserOrderStatsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((TotalOrders: 0, PendingOrders: 0, InDeliveryOrders: 0, CompletedOrders: 0));
        _loyaltyRepo.GetCurrentLoyaltyPoints(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(0);
        _orderRepo.GetRecentOrdersForUserAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<Order>)[]);
    }

    private static Order CreateOrder(int id, decimal price, OrderStatus status, DateTime createdAt) => new()
    {
        Id = id,
        OrderNumber = $"ORD-{id}",
        RecipientFullName = "Marko Markovic",
        RecipientPhoneNumber = "0611234567",
        OrderAddress = "Nemanjina 1",
        City = "Beograd",
        ZipCode = "11000",
        UserId = UserId,
        OrderPrice = price,
        OrderStatus = status,
        CreatedAt = createdAt
    };

    [Fact]
    public async Task Handle_AggregatesTotalOrdersUpcomingOrdersAndLoyaltyPoints()
    {
        _orderRepo.GetUserOrderStatsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((TotalOrders: 12, PendingOrders: 2, InDeliveryOrders: 3, CompletedOrders: 7));
        _loyaltyRepo.GetCurrentLoyaltyPoints(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(850);

        var response = await _sut.Handle(new GetUserDashboardQuery(UserId));

        response.TotalOrders.Should().Be(12);
        response.UpcomingOrders.Should().Be(5); 
        response.LoyaltyPoints.Should().Be(850);
    }

    [Fact]
    public async Task Handle_MapsRecentOrdersToUserRecentOrderDto()
    {
        var createdAt = new DateTime(2026, 3, 1, 10, 0, 0);
        var order = CreateOrder(1, price: 1500m, OrderStatus.Completed, createdAt);

        _orderRepo.GetRecentOrdersForUserAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<Order>)[order]);

        var response = await _sut.Handle(new GetUserDashboardQuery(UserId));

        response.RecentOrders.Should().ContainSingle().Which.Should().BeEquivalentTo(new
        {
            Id = 1,
            OrderNumber = order.OrderNumber,
            OrderPrice = 1500m,
            OrderStatus = OrderStatus.Completed,
            CreatedAt = createdAt
        });
    }

    [Fact]
    public async Task Handle_WhenNoRecentOrders_ReturnsEmptyRecentOrdersList()
    {
        _orderRepo.GetRecentOrdersForUserAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<Order>)[]);

        var response = await _sut.Handle(new GetUserDashboardQuery(UserId));

        response.RecentOrders.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_PassesQueryUserIdToAllRepositoryCalls()
    {
        await _sut.Handle(new GetUserDashboardQuery(UserId));

        await _orderRepo.Received(1).GetUserOrderStatsAsync(UserId, Arg.Any<CancellationToken>());
        await _loyaltyRepo.Received(1).GetCurrentLoyaltyPoints(UserId, Arg.Any<CancellationToken>());
        await _orderRepo.Received(1).GetRecentOrdersForUserAsync(UserId, 5, Arg.Any<CancellationToken>());
    }
}
