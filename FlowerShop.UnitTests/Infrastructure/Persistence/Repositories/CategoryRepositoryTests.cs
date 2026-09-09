using FlowerShop.Infrastructure.Persistence.Repositories;
using FluentAssertions;

namespace FlowerShop.UnitTests.Infrastructure.Persistence.Repositories;

public class CategoryRepositoryTests : RepositoryTestBase
{
    private readonly CategoryRepository _sut;

    public CategoryRepositoryTests()
    {
        _sut = new CategoryRepository(Context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategoriesMappedToDto()
    {
        var category = TestEntityFactory.CreateCategory("Buketi za venčanje");
        Context.Categories.Add(category);
        await Context.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        result.Should().Contain(c => c.Id == category.Id && c.Name == category.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ReturnsCategory()
    {
        var category = TestEntityFactory.CreateCategory();
        Context.Categories.Add(category);
        await Context.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(category.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(category.Id);
        result.Name.Should().Be(category.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(-1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_WhenCategoryExists_ReturnsTrue()
    {
        var category = TestEntityFactory.CreateCategory();
        Context.Categories.Add(category);
        await Context.SaveChangesAsync();

        var result = await _sut.ExistsAsync(category.Id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenCategoryDoesNotExist_ReturnsFalse()
    {
        var result = await _sut.ExistsAsync(-1);

        result.Should().BeFalse();
    }
}
