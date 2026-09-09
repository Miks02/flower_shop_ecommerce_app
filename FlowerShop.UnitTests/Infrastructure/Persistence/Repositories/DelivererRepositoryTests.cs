using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Infrastructure.Persistence.Repositories;
using FluentAssertions;

namespace FlowerShop.UnitTests.Infrastructure.Persistence.Repositories;

public class DelivererRepositoryTests : RepositoryTestBase
{
    private readonly DelivererRepository _sut;

    public DelivererRepositoryTests()
    {
        _sut = new DelivererRepository(Context);
    }

    private async Task<Deliverer> SeedDelivererAsync(
        string firstName = "Marko",
        string lastName = "Markovic",
        VehicleType vehicleType = VehicleType.Car,
        DelivererStatus status = DelivererStatus.Available)
    {
        var user = TestEntityFactory.CreateUser(firstName: firstName, lastName: lastName);
        var deliverer = TestEntityFactory.CreateDeliverer(user, vehicleType, status);
        Context.Users.Add(user);
        Context.Deliverers.Add(deliverer);
        await Context.SaveChangesAsync();
        return deliverer;
    }

    [Fact]
    public async Task GetPagedDeliverersAsync_ReturnsMatchingDeliverersMappedToDto()
    {
        var deliverer = await SeedDelivererAsync(firstName: "Ana", lastName: "Anic");

        var result = await _sut.GetPagedDeliverersAsync(null, null, null, null, 1, 10);

        result.Items.Should().Contain(d => d.Id == deliverer.Id && d.FirstName == "Ana" && d.LastName == "Anic");
    }

    [Fact]
    public async Task GetPagedDeliverersAsync_FiltersByVehicleType()
    {
        var bicycleDeliverer = await SeedDelivererAsync(vehicleType: VehicleType.Bicycle);
        var carDeliverer = await SeedDelivererAsync(vehicleType: VehicleType.Car);

        var result = await _sut.GetPagedDeliverersAsync(null, null, VehicleType.Bicycle, null, 1, 10);

        result.Items.Select(d => d.Id).Should().Contain(bicycleDeliverer.Id);
        result.Items.Select(d => d.Id).Should().NotContain(carDeliverer.Id);
    }

    [Fact]
    public async Task GetPagedDeliverersAsync_FiltersByDelivererStatus()
    {
        var availableDeliverer = await SeedDelivererAsync(status: DelivererStatus.Available);
        var onDutyDeliverer = await SeedDelivererAsync(status: DelivererStatus.OnDuty);

        var result = await _sut.GetPagedDeliverersAsync(null, null, null, DelivererStatus.OnDuty, 1, 10);

        result.Items.Select(d => d.Id).Should().Contain(onDutyDeliverer.Id);
        result.Items.Select(d => d.Id).Should().NotContain(availableDeliverer.Id);
    }

    [Fact]
    public async Task GetPagedDeliverersAsync_FiltersBySearchTerm()
    {
        var matching = await SeedDelivererAsync(firstName: "Jovana", lastName: "Jovanovic");
        var nonMatching = await SeedDelivererAsync(firstName: "Petar", lastName: "Petrovic");

        var result = await _sut.GetPagedDeliverersAsync("Jovana", null, null, null, 1, 10);

        result.Items.Select(d => d.Id).Should().Contain(matching.Id);
        result.Items.Select(d => d.Id).Should().NotContain(nonMatching.Id);
    }

    [Fact]
    public async Task GetPagedDeliverersAsync_RespectsPagination()
    {
        await SeedDelivererAsync();
        await SeedDelivererAsync();
        await SeedDelivererAsync();

        var result = await _sut.GetPagedDeliverersAsync(null, null, null, null, 1, 2);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task GetStatisticsAsync_ReturnsAggregatedStatistics()
    {
        var deliverer = await SeedDelivererAsync(vehicleType: VehicleType.Scooter, status: DelivererStatus.OnDuty);
        var order = TestEntityFactory.CreateOrder(deliverer.User, deliverer);
        Context.Orders.Add(order);
        await Context.SaveChangesAsync();
        var review = TestEntityFactory.CreateServiceReview(order, deliverer.User, rating: 4);
        Context.ServiceReviews.Add(review);
        await Context.SaveChangesAsync();

        var result = await _sut.GetStatisticsAsync();

        result.TotalCount.Should().Be(1);
        result.OnDutyCount.Should().Be(1);
        result.ScooterCount.Should().Be(1);
        result.TotalRating.Should().Be(4);
    }

    [Fact]
    public async Task GetAvailableDeliverersListAsync_ReturnsOnlyDeliverersThatAreNotUnavailable()
    {
        var available = await SeedDelivererAsync(status: DelivererStatus.Available);
        var onDuty = await SeedDelivererAsync(status: DelivererStatus.OnDuty);
        var unavailable = await SeedDelivererAsync(status: DelivererStatus.Unavailable);

        var result = await _sut.GetAvailableDeliverersListAsync();

        var ids = result.Select(d => d.Id).ToList();
        ids.Should().Contain(available.Id);
        ids.Should().Contain(onDuty.Id);
        ids.Should().NotContain(unavailable.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenDelivererExists_ReturnsDelivererWithUser()
    {
        var deliverer = await SeedDelivererAsync();

        var result = await _sut.GetByIdAsync(deliverer.Id);

        result.Should().NotBeNull();
        result!.User.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenDelivererDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync("unknown-id");

        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_WhenDelivererExists_ReturnsTrue()
    {
        var deliverer = await SeedDelivererAsync();

        var result = await _sut.ExistsAsync(deliverer.Id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenDelivererDoesNotExist_ReturnsFalse()
    {
        var result = await _sut.ExistsAsync("unknown-id");

        result.Should().BeFalse();
    }
}
