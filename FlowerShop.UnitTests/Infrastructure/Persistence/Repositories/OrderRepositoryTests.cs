using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.UnitTests.Infrastructure.Persistence.Repositories;

public class OrderRepositoryTests : RepositoryTestBase
{
    private readonly OrderRepository _sut;

    public OrderRepositoryTests()
    {
        _sut = new OrderRepository(Context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ReturnsOrderWithRelatedData()
    {
        var user = TestEntityFactory.CreateUser();
        var order = TestEntityFactory.CreateOrder(user);
        Context.Users.Add(user);
        Context.Orders.Add(order);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var result = await _sut.GetByIdAsync(order.Id);

        result.Should().NotBeNull();
        result!.User.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(-1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdForUserAsync_WhenOrderBelongsToUser_ReturnsOrder()
    {
        var user = TestEntityFactory.CreateUser();
        var order = TestEntityFactory.CreateOrder(user);
        Context.Users.Add(user);
        Context.Orders.Add(order);
        await Context.SaveChangesAsync();

        var result = await _sut.GetByIdForUserAsync(order.Id, user.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task GetByIdForUserAsync_WhenOrderBelongsToAnotherUser_ReturnsNull()
    {
        var owner = TestEntityFactory.CreateUser();
        var otherUser = TestEntityFactory.CreateUser();
        var order = TestEntityFactory.CreateOrder(owner);
        Context.Users.AddRange(owner, otherUser);
        Context.Orders.Add(order);
        await Context.SaveChangesAsync();

        var result = await _sut.GetByIdForUserAsync(order.Id, otherUser.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPagedOrdersForUserAsync_ReturnsOnlyOrdersOfGivenUser()
    {
        var user = TestEntityFactory.CreateUser();
        var otherUser = TestEntityFactory.CreateUser();
        var userOrder = TestEntityFactory.CreateOrder(user);
        var otherOrder = TestEntityFactory.CreateOrder(otherUser);
        Context.Users.AddRange(user, otherUser);
        Context.Orders.AddRange(userOrder, otherOrder);
        await Context.SaveChangesAsync();

        var result = await _sut.GetPagedOrdersForUserAsync(user.Id, null, null, null, 1, 10);

        result.Items.Select(o => o.Id).Should().Contain(userOrder.Id);
        result.Items.Select(o => o.Id).Should().NotContain(otherOrder.Id);
    }

    [Fact]
    public async Task GetPagedOrdersForUserAsync_FiltersByStatus()
    {
        var user = TestEntityFactory.CreateUser();
        var pending = TestEntityFactory.CreateOrder(user, orderStatus: OrderStatus.Pending);
        var completed = TestEntityFactory.CreateOrder(user, orderStatus: OrderStatus.Completed);
        Context.Users.Add(user);
        Context.Orders.AddRange(pending, completed);
        await Context.SaveChangesAsync();

        var result = await _sut.GetPagedOrdersForUserAsync(user.Id, null, null, OrderStatus.Completed, 1, 10);

        result.Items.Select(o => o.Id).Should().Contain(completed.Id);
        result.Items.Select(o => o.Id).Should().NotContain(pending.Id);
    }

    [Fact]
    public async Task GetPagedOrdersForUserAsync_FiltersBySearchTerm()
    {
        var user = TestEntityFactory.CreateUser();
        var matching = TestEntityFactory.CreateOrder(user, recipientFullName: "Jovana Jovanovic");
        var nonMatching = TestEntityFactory.CreateOrder(user, recipientFullName: "Petar Petrovic");
        Context.Users.Add(user);
        Context.Orders.AddRange(matching, nonMatching);
        await Context.SaveChangesAsync();

        var result = await _sut.GetPagedOrdersForUserAsync(user.Id, "Jovana", null, null, 1, 10);

        result.Items.Select(o => o.Id).Should().Contain(matching.Id);
        result.Items.Select(o => o.Id).Should().NotContain(nonMatching.Id);
    }

    [Fact]
    public async Task GetUserOrderStatsAsync_ReturnsAggregatedCountsForUser()
    {
        var user = TestEntityFactory.CreateUser();
        var pending = TestEntityFactory.CreateOrder(user, orderStatus: OrderStatus.Pending);
        var inTransit = TestEntityFactory.CreateOrder(user, orderStatus: OrderStatus.Confirmed, deliveryStatus: DeliveryStatus.InTransit);
        var completed = TestEntityFactory.CreateOrder(user, orderStatus: OrderStatus.Completed);
        Context.Users.Add(user);
        Context.Orders.AddRange(pending, inTransit, completed);
        await Context.SaveChangesAsync();

        var result = await _sut.GetUserOrderStatsAsync(user.Id);

        result.TotalOrders.Should().Be(3);
        result.PendingOrders.Should().Be(1);
        result.InDeliveryOrders.Should().Be(1);
        result.CompletedOrders.Should().Be(1);
    }

    [Fact]
    public async Task GetPagedOrdersForAdminAsync_FiltersByAssignmentStatus()
    {
        var user = TestEntityFactory.CreateUser();
        var deliverer = TestEntityFactory.CreateDeliverer(TestEntityFactory.CreateUser());
        var assigned = TestEntityFactory.CreateOrder(user, deliverer);
        var unassigned = TestEntityFactory.CreateOrder(user);
        Context.Users.Add(user);
        Context.Users.Add(deliverer.User);
        Context.Deliverers.Add(deliverer);
        Context.Orders.AddRange(assigned, unassigned);
        await Context.SaveChangesAsync();

        var result = await _sut.GetPagedOrdersForAdminAsync(null, null, null, null, true, 1, 10);

        result.Items.Select(o => o.Id).Should().Contain(assigned.Id);
        result.Items.Select(o => o.Id).Should().NotContain(unassigned.Id);
    }

    [Fact]
    public async Task GetPagedOrdersForDeliverersAsync_ReturnsOnlyOrdersAssignedToDeliverer()
    {
        var user = TestEntityFactory.CreateUser();
        var delivererOne = TestEntityFactory.CreateDeliverer(TestEntityFactory.CreateUser());
        var delivererTwo = TestEntityFactory.CreateDeliverer(TestEntityFactory.CreateUser());
        var orderOne = TestEntityFactory.CreateOrder(user, delivererOne);
        var orderTwo = TestEntityFactory.CreateOrder(user, delivererTwo);
        Context.Users.AddRange(user, delivererOne.User, delivererTwo.User);
        Context.Deliverers.AddRange(delivererOne, delivererTwo);
        Context.Orders.AddRange(orderOne, orderTwo);
        await Context.SaveChangesAsync();

        var result = await _sut.GetPagedOrdersForDeliverersAsync(delivererOne.Id, null, null, null, null, 1, 10);

        result.Items.Select(o => o.Id).Should().Contain(orderOne.Id);
        result.Items.Select(o => o.Id).Should().NotContain(orderTwo.Id);
    }

    [Fact]
    public async Task GetAdminOrderStatsAsync_ReturnsAggregatedCounts()
    {
        var user = TestEntityFactory.CreateUser();
        var pending = TestEntityFactory.CreateOrder(user, orderStatus: OrderStatus.Pending);
        var completed = TestEntityFactory.CreateOrder(user, orderStatus: OrderStatus.Completed);
        var cancelled = TestEntityFactory.CreateOrder(user, orderStatus: OrderStatus.Cancelled);
        Context.Users.Add(user);
        Context.Orders.AddRange(pending, completed, cancelled);
        await Context.SaveChangesAsync();

        var result = await _sut.GetAdminOrderStatsAsync();

        result.TotalOrders.Should().Be(3);
        result.PendingOrders.Should().Be(1);
        result.CompletedOrders.Should().Be(1);
        result.ActiveOrders.Should().Be(1);
    }

    [Fact]
    public async Task GetTodaySalesStatsAsync_SumsPriceForTodaysNonCancelledOrders()
    {
        var user = TestEntityFactory.CreateUser();
        var todayOrder = TestEntityFactory.CreateOrder(user, orderPrice: 1500m, createdAt: DateTime.UtcNow);
        var cancelledToday = TestEntityFactory.CreateOrder(user, orderPrice: 1000m, orderStatus: OrderStatus.Cancelled, createdAt: DateTime.UtcNow);
        var yesterdayOrder = TestEntityFactory.CreateOrder(user, orderPrice: 2000m, createdAt: DateTime.UtcNow.AddDays(-1));
        Context.Users.Add(user);
        Context.Orders.AddRange(todayOrder, cancelledToday, yesterdayOrder);
        await Context.SaveChangesAsync();

        var result = await _sut.GetTodaySalesStatsAsync();

        result.TotalSales.Should().Be(1500m);
        result.NewOrdersCount.Should().Be(1);
    }

    [Fact]
    public async Task GetRecentOrdersAsync_ReturnsOrdersOrderedByCreatedAtDescending()
    {
        var user = TestEntityFactory.CreateUser();
        var older = TestEntityFactory.CreateOrder(user, createdAt: DateTime.UtcNow.AddDays(-2));
        var newer = TestEntityFactory.CreateOrder(user, createdAt: DateTime.UtcNow.AddDays(-1));
        Context.Users.Add(user);
        Context.Orders.AddRange(older, newer);
        await Context.SaveChangesAsync();

        var result = await _sut.GetRecentOrdersAsync(1);

        result.Should().ContainSingle();
        result[0].Id.Should().Be(newer.Id);
    }

    [Fact]
    public async Task GetRecentOrdersForUserAsync_ReturnsOnlyOrdersOfGivenUserOrderedByCreatedAtDescending()
    {
        var user = TestEntityFactory.CreateUser();
        var otherUser = TestEntityFactory.CreateUser();
        var userOrder = TestEntityFactory.CreateOrder(user, createdAt: DateTime.UtcNow.AddDays(-1));
        var otherOrder = TestEntityFactory.CreateOrder(otherUser, createdAt: DateTime.UtcNow);
        Context.Users.AddRange(user, otherUser);
        Context.Orders.AddRange(userOrder, otherOrder);
        await Context.SaveChangesAsync();

        var result = await _sut.GetRecentOrdersForUserAsync(user.Id, 5);

        result.Should().ContainSingle();
        result[0].Id.Should().Be(userOrder.Id);
    }

    [Fact]
    public async Task GetDelivererOrderStatsAsync_ReturnsAggregatedStatisticsForDeliverer()
    {
        var deliverer = TestEntityFactory.CreateDeliverer(TestEntityFactory.CreateUser());
        var user = TestEntityFactory.CreateUser();
        var active = TestEntityFactory.CreateOrder(user, deliverer, orderStatus: OrderStatus.Confirmed);
        var completed = TestEntityFactory.CreateOrder(user, deliverer, orderStatus: OrderStatus.Completed);
        Context.Users.AddRange(deliverer.User, user);
        Context.Deliverers.Add(deliverer);
        Context.Orders.AddRange(active, completed);
        await Context.SaveChangesAsync();
        Context.ServiceReviews.Add(TestEntityFactory.CreateServiceReview(completed, user, rating: 5));
        await Context.SaveChangesAsync();

        var result = await _sut.GetDelivererOrderStatsAsync(deliverer.Id);

        result.TotalDeliveries.Should().Be(2);
        result.ActiveDeliveries.Should().Be(1);
        result.CompletedDeliveries.Should().Be(1);
        result.AverageRating.Should().Be(5);
    }

    [Fact]
    public async Task GetDelivererOrderStatsAsync_WhenNoReviewsExist_ReturnsZeroAverageRating()
    {
        var deliverer = TestEntityFactory.CreateDeliverer(TestEntityFactory.CreateUser());
        var user = TestEntityFactory.CreateUser();
        var order = TestEntityFactory.CreateOrder(user, deliverer);
        Context.Users.AddRange(deliverer.User, user);
        Context.Deliverers.Add(deliverer);
        Context.Orders.Add(order);
        await Context.SaveChangesAsync();

        var result = await _sut.GetDelivererOrderStatsAsync(deliverer.Id);

        result.AverageRating.Should().Be(0);
    }

    [Fact]
    public void GetItemById_WhenItemExists_ReturnsItem()
    {
        var user = TestEntityFactory.CreateUser();
        var order = TestEntityFactory.CreateOrder(user);
        var item = TestEntityFactory.CreateOrderItem(order);
        Context.Users.Add(user);
        Context.Orders.Add(order);
        Context.OrderItems.Add(item);
        Context.SaveChanges();

        var result = _sut.GetItemById(item.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(item.Id);
    }

    [Fact]
    public void GetItemById_WhenItemDoesNotExist_ReturnsNull()
    {
        var result = _sut.GetItemById(-1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task RemoveItem_RemovesOrderItem()
    {
        var user = TestEntityFactory.CreateUser();
        var order = TestEntityFactory.CreateOrder(user);
        var item = TestEntityFactory.CreateOrderItem(order);
        Context.Users.Add(user);
        Context.Orders.Add(order);
        Context.OrderItems.Add(item);
        await Context.SaveChangesAsync();

        _sut.RemoveItem(item);
        await Context.SaveChangesAsync();

        var stored = await Context.OrderItems.AsNoTracking().FirstOrDefaultAsync(i => i.Id == item.Id);
        stored.Should().BeNull();
    }
}
