using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.Domain.Entities.LoyaltyTransactions;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Infrastructure.Persistence.Repositories;
using FluentAssertions;

namespace FlowerShop.UnitTests.Infrastructure.Persistence.Repositories;

public class LoyaltyTransactionRepositoryTests : RepositoryTestBase
{
    private readonly LoyaltyTransactionRepository _sut;

    public LoyaltyTransactionRepositoryTests()
    {
        _sut = new LoyaltyTransactionRepository(Context);
    }

    private async Task<(User User, Order Order)> SeedUserAndOrderAsync()
    {
        var user = TestEntityFactory.CreateUser();
        var order = TestEntityFactory.CreateOrder(user);
        Context.Users.Add(user);
        Context.Orders.Add(order);
        await Context.SaveChangesAsync();
        return (user, order);
    }

    [Fact]
    public async Task GetMostRecentLoyaltyTransaction_ReturnsTransactionWithLatestDate()
    {
        var (user, order) = await SeedUserAndOrderAsync();
        var older = TestEntityFactory.CreateLoyaltyTransaction(user, order, transactionDate: DateTime.UtcNow.AddDays(-2), currentPoints: 50);
        var newer = TestEntityFactory.CreateLoyaltyTransaction(user, order, transactionDate: DateTime.UtcNow.AddDays(-1), currentPoints: 80);
        Context.LoyaltyTransactions.AddRange(older, newer);
        await Context.SaveChangesAsync();

        var result = await _sut.GetMostRecentLoyaltyTransaction(user.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(newer.Id);
    }

    [Fact]
    public async Task GetMostRecentLoyaltyTransaction_WhenNoneExist_ReturnsNull()
    {
        var user = TestEntityFactory.CreateUser();
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var result = await _sut.GetMostRecentLoyaltyTransaction(user.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCurrentLoyaltyPoints_ReturnsPointsFromMostRecentTransaction()
    {
        var (user, order) = await SeedUserAndOrderAsync();
        var older = TestEntityFactory.CreateLoyaltyTransaction(user, order, transactionDate: DateTime.UtcNow.AddDays(-2), currentPoints: 50);
        var newer = TestEntityFactory.CreateLoyaltyTransaction(user, order, transactionDate: DateTime.UtcNow.AddDays(-1), currentPoints: 80);
        Context.LoyaltyTransactions.AddRange(older, newer);
        await Context.SaveChangesAsync();

        var result = await _sut.GetCurrentLoyaltyPoints(user.Id);

        result.Should().Be(80);
    }

    [Fact]
    public async Task GetCurrentLoyaltyPoints_WhenNoneExist_ReturnsZero()
    {
        var user = TestEntityFactory.CreateUser();
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var result = await _sut.GetCurrentLoyaltyPoints(user.Id);

        result.Should().Be(0);
    }

    [Fact]
    public async Task GetLastLoyaltyTransactionByOrderId_ReturnsLatestTransactionForOrder()
    {
        var (user, order) = await SeedUserAndOrderAsync();
        var older = TestEntityFactory.CreateLoyaltyTransaction(user, order, transactionDate: DateTime.UtcNow.AddDays(-2));
        var newer = TestEntityFactory.CreateLoyaltyTransaction(user, order, transactionDate: DateTime.UtcNow.AddDays(-1));
        Context.LoyaltyTransactions.AddRange(older, newer);
        await Context.SaveChangesAsync();

        var result = await _sut.GetLastLoyaltyTransactionByOrderId(order.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(newer.Id);
    }

    [Fact]
    public async Task GetLastLoyaltyTransactionByOrderId_WhenNoneExist_ReturnsNull()
    {
        var result = await _sut.GetLastLoyaltyTransactionByOrderId(-1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllSpentLoyaltyPoints_SumsPreviousPointsForRedeemedTransactions()
    {
        var (user, order) = await SeedUserAndOrderAsync();
        var redeemed = TestEntityFactory.CreateLoyaltyTransaction(user, order, transactionType: TransactionType.Redeemed, previousPoints: 30);
        var earned = TestEntityFactory.CreateLoyaltyTransaction(user, order, transactionType: TransactionType.Earned, previousPoints: 100);
        Context.LoyaltyTransactions.AddRange(redeemed, earned);
        await Context.SaveChangesAsync();

        var result = await _sut.GetAllSpentLoyaltyPoints();

        result.Should().Be(30);
    }

    [Fact]
    public async Task GetAllSpentLoyaltyPointsByUserId_SumsPreviousPointsForRedeemedTransactionsOfGivenUser()
    {
        var (userOne, orderOne) = await SeedUserAndOrderAsync();
        var (userTwo, orderTwo) = await SeedUserAndOrderAsync();
        var userOneRedeemed = TestEntityFactory.CreateLoyaltyTransaction(userOne, orderOne, transactionType: TransactionType.Redeemed, previousPoints: 40);
        var userTwoRedeemed = TestEntityFactory.CreateLoyaltyTransaction(userTwo, orderTwo, transactionType: TransactionType.Redeemed, previousPoints: 60);
        Context.LoyaltyTransactions.AddRange(userOneRedeemed, userTwoRedeemed);
        await Context.SaveChangesAsync();

        var result = await _sut.GetAllSpentLoyaltyPointsByUserId(userOne.Id);

        result.Should().Be(40);
    }
}
