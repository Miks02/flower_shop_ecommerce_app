using FlowerShop.Domain.Enums;
using FlowerShop.Infrastructure.Persistence.Repositories;
using FluentAssertions;

namespace FlowerShop.UnitTests.Infrastructure.Persistence.Repositories;

public class FlowerRepositoryTests : RepositoryTestBase
{
    private readonly FlowerRepository _sut;

    public FlowerRepositoryTests()
    {
        _sut = new FlowerRepository(Context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllFlowersMappedToDto()
    {
        var flower = TestEntityFactory.CreateFlower(color: "Tirkizna");
        Context.Flowers.Add(flower);
        await Context.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        result.Should().Contain(f =>
            f.Id == flower.Id &&
            f.Name == flower.Name &&
            f.Stock == flower.Stock &&
            f.Color == flower.Color);
    }

    [Fact]
    public async Task GetFlowersUsedInProductsAsync_ReturnsOnlyFlowersUsedInAProduct()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user);
        var usedFlower = TestEntityFactory.CreateFlower(color: "Magenta");
        var unusedFlower = TestEntityFactory.CreateFlower(color: "Zelena");
        Context.Products.Add(product);
        Context.Flowers.AddRange(usedFlower, unusedFlower);
        Context.ProductFlowers.Add(TestEntityFactory.CreateProductFlower(product, usedFlower));
        await Context.SaveChangesAsync();

        var result = await _sut.GetFlowersUsedInProductsAsync();

        result.Should().Contain(f => f.Id == usedFlower.Id);
        result.Should().NotContain(f => f.Id == unusedFlower.Id);
    }

    [Fact]
    public async Task GetFlowersByIdsAsync_ReturnsMatchingFlowers()
    {
        var flowerOne = TestEntityFactory.CreateFlower(color: "Bordo");
        var flowerTwo = TestEntityFactory.CreateFlower(color: "Krem");
        Context.Flowers.AddRange(flowerOne, flowerTwo);
        await Context.SaveChangesAsync();

        var result = await _sut.GetFlowersByIdsAsync([flowerOne.Id, flowerTwo.Id]);

        result.Select(f => f.Id).Should().BeEquivalentTo([flowerOne.Id, flowerTwo.Id]);
    }

    [Fact]
    public async Task GetInvalidFlowerIdsAsync_WhenIdsListIsEmpty_ReturnsEmptyList()
    {
        var result = await _sut.GetInvalidFlowerIdsAsync([]);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetInvalidFlowerIdsAsync_WhenSomeIdsAreInvalid_ReturnsOnlyInvalidIds()
    {
        var flower = TestEntityFactory.CreateFlower(color: "Ćilibar");
        Context.Flowers.Add(flower);
        await Context.SaveChangesAsync();

        var result = await _sut.GetInvalidFlowerIdsAsync([flower.Id, -1]);

        result.Should().BeEquivalentTo([-1]);
    }

    [Fact]
    public async Task GetByIdAsync_WhenFlowerExists_ReturnsFlower()
    {
        var flower = TestEntityFactory.CreateFlower(color: "Indigo");
        Context.Flowers.Add(flower);
        await Context.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(flower.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(flower.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenFlowerDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(-1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_WhenFlowerWithSameNameColorAndCategoryExists_ReturnsTrue()
    {
        var flower = TestEntityFactory.CreateFlower(name: "Zumbul", color: "Roze", flowerCategory: FlowerCategory.Artificial);
        Context.Flowers.Add(flower);
        await Context.SaveChangesAsync();

        var result = await _sut.ExistsAsync("Zumbul", "Roze", FlowerCategory.Artificial);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenNoMatchingFlowerExists_ReturnsFalse()
    {
        var result = await _sut.ExistsAsync("Nepostojeći", "Nepostojeća", FlowerCategory.Fresh);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsUsedInProductsAsync_WhenFlowerIsUsedInProduct_ReturnsTrue()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user);
        var flower = TestEntityFactory.CreateFlower(color: "Braon");
        Context.Products.Add(product);
        Context.Flowers.Add(flower);
        Context.ProductFlowers.Add(TestEntityFactory.CreateProductFlower(product, flower));
        await Context.SaveChangesAsync();

        var result = await _sut.IsUsedInProductsAsync(flower.Id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsUsedInProductsAsync_WhenFlowerIsNotUsedInAnyProduct_ReturnsFalse()
    {
        var flower = TestEntityFactory.CreateFlower(color: "Siva");
        Context.Flowers.Add(flower);
        await Context.SaveChangesAsync();

        var result = await _sut.IsUsedInProductsAsync(flower.Id);

        result.Should().BeFalse();
    }
}
