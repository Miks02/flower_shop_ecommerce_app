using FlowerShop.Infrastructure.Persistence.Repositories;
using FluentAssertions;

namespace FlowerShop.UnitTests.Infrastructure.Persistence.Repositories;

public class OccasionRepositoryTests : RepositoryTestBase
{
    private readonly OccasionRepository _sut;

    public OccasionRepositoryTests()
    {
        _sut = new OccasionRepository(Context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllOccasionsMappedToDto()
    {
        var occasion = TestEntityFactory.CreateOccasion("Godišnjica braka");
        Context.Occasions.Add(occasion);
        await Context.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        result.Should().Contain(o => o.Id == occasion.Id && o.Name == occasion.Name);
    }

    [Fact]
    public async Task GetInvalidOccasionIdsAsync_WhenAllIdsAreValid_ReturnsEmptyList()
    {
        var occasion = TestEntityFactory.CreateOccasion();
        Context.Occasions.Add(occasion);
        await Context.SaveChangesAsync();

        var result = await _sut.GetInvalidOccasionIdsAsync([occasion.Id]);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetInvalidOccasionIdsAsync_WhenSomeIdsAreInvalid_ReturnsOnlyInvalidIds()
    {
        var occasion = TestEntityFactory.CreateOccasion();
        Context.Occasions.Add(occasion);
        await Context.SaveChangesAsync();

        var result = await _sut.GetInvalidOccasionIdsAsync([occasion.Id, -1, -2]);

        result.Should().BeEquivalentTo([-1, -2]);
    }

    [Fact]
    public async Task GetOccasionsByIdsAsync_ReturnsMatchingOccasions()
    {
        var occasionOne = TestEntityFactory.CreateOccasion();
        var occasionTwo = TestEntityFactory.CreateOccasion();
        var occasionThree = TestEntityFactory.CreateOccasion();
        Context.Occasions.AddRange(occasionOne, occasionTwo, occasionThree);
        await Context.SaveChangesAsync();

        var result = await _sut.GetOccasionsByIdsAsync([occasionOne.Id, occasionTwo.Id]);

        result.Should().HaveCount(2);
        result.Select(o => o.Id).Should().BeEquivalentTo([occasionOne.Id, occasionTwo.Id]);
    }

    [Fact]
    public async Task GetOccasionsByIdsAsync_WhenNoIdsMatch_ReturnsEmptyList()
    {
        var result = await _sut.GetOccasionsByIdsAsync([-1, -2]);

        result.Should().BeEmpty();
    }
}
