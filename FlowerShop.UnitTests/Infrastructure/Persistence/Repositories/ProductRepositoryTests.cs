using FlowerShop.Domain.Entities.Products;
using FlowerShop.Infrastructure.Persistence.Repositories;
using FluentAssertions;

namespace FlowerShop.UnitTests.Infrastructure.Persistence.Repositories;

public class ProductRepositoryTests : InMemoryRepositoryTestBase
{
    private readonly ProductRepository _sut;

    public ProductRepositoryTests()
    {
        _sut = new ProductRepository(Context);
    }

    private void AddReview(Product product, decimal rating = 5)
    {
        Context.ProductReviews.Add(TestEntityFactory.CreateProductReview(product, product.User, rating));
    }

    [Fact]
    public async Task GetPagedProductsAsync_WithoutFilters_ReturnsMatchingProductsMappedToDto()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user, name: "Buket ruža");
        Context.Products.Add(product);
        AddReview(product);
        await Context.SaveChangesAsync();

        var result = await _sut.GetPagedProductsAsync(null, null, null, false, 1, 10, []);

        result.Items.Should().Contain(p => p.Id == product.Id && p.Name == "Buket ruža");
    }

    [Fact]
    public async Task GetPagedProductsAsync_WhenProductHasNoReviews_ReturnsNullAverageRating()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user);
        Context.Products.Add(product);
        await Context.SaveChangesAsync();

        var result = await _sut.GetPagedProductsAsync(null, null, null, false, 1, 10, []);

        result.Items.Should().Contain(p => p.Id == product.Id && p.AverageRating == null);
    }

    [Fact]
    public async Task GetPagedProductsAsync_Overload_WhenProductHasNoReviews_ReturnsNullAverageRating()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user);
        Context.Products.Add(product);
        await Context.SaveChangesAsync();

        var result = await _sut.GetPagedProductsAsync(null, 1, 10, [], [], [], 0);

        result.Items.Should().Contain(p => p.Id == product.Id && p.AverageRating == null);
    }

    [Fact]
    public async Task GetPagedProductsAsync_FiltersByCategoryId()
    {
        var user = TestEntityFactory.CreateUser();
        var categoryOne = TestEntityFactory.CreateCategory();
        var categoryTwo = TestEntityFactory.CreateCategory();
        var matching = TestEntityFactory.CreateProduct(categoryOne, user);
        var nonMatching = TestEntityFactory.CreateProduct(categoryTwo, user);
        Context.Products.AddRange(matching, nonMatching);
        AddReview(matching);
        AddReview(nonMatching);
        await Context.SaveChangesAsync();

        var result = await _sut.GetPagedProductsAsync(null, null, categoryOne.Id, false, 1, 10, []);

        result.Items.Select(p => p.Id).Should().Contain(matching.Id);
        result.Items.Select(p => p.Id).Should().NotContain(nonMatching.Id);
    }

    [Fact]
    public async Task GetPagedProductsAsync_WhenIsDeletedIsTrue_ReturnsOnlyDeletedProducts()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var deleted = TestEntityFactory.CreateProduct(category, user, isDeleted: true);
        var active = TestEntityFactory.CreateProduct(category, user, isDeleted: false);
        Context.Products.AddRange(deleted, active);
        AddReview(deleted);
        AddReview(active);
        await Context.SaveChangesAsync();

        var result = await _sut.GetPagedProductsAsync(null, null, null, true, 1, 10, []);

        result.Items.Select(p => p.Id).Should().Contain(deleted.Id);
        result.Items.Select(p => p.Id).Should().NotContain(active.Id);
    }

    [Fact]
    public async Task GetPagedProductsAsync_FiltersBySearchTerm()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var matching = TestEntityFactory.CreateProduct(category, user, name: "Prolećni buket");
        var nonMatching = TestEntityFactory.CreateProduct(category, user, name: "Jesenji aranžman");
        Context.Products.AddRange(matching, nonMatching);
        AddReview(matching);
        AddReview(nonMatching);
        await Context.SaveChangesAsync();

        var result = await _sut.GetPagedProductsAsync("Prolećni", null, null, false, 1, 10, []);

        result.Items.Select(p => p.Id).Should().Contain(matching.Id);
        result.Items.Select(p => p.Id).Should().NotContain(nonMatching.Id);
    }

    [Fact]
    public async Task GetPagedProductsAsync_FiltersByOccasionIds()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var occasion = TestEntityFactory.CreateOccasion();
        var matching = TestEntityFactory.CreateProduct(category, user);
        matching.Occasions.Add(occasion);
        var nonMatching = TestEntityFactory.CreateProduct(category, user);
        Context.Products.AddRange(matching, nonMatching);
        AddReview(matching);
        AddReview(nonMatching);
        await Context.SaveChangesAsync();

        var result = await _sut.GetPagedProductsAsync(null, null, null, false, 1, 10, [occasion.Id]);

        result.Items.Select(p => p.Id).Should().Contain(matching.Id);
        result.Items.Select(p => p.Id).Should().NotContain(nonMatching.Id);
    }

    [Fact]
    public async Task GetPagedProductsAsync_SortsByPriceAscending()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var cheap = TestEntityFactory.CreateProduct(category, user, price: 500m);
        var expensive = TestEntityFactory.CreateProduct(category, user, price: 5000m);
        Context.Products.AddRange(expensive, cheap);
        AddReview(cheap);
        AddReview(expensive);
        await Context.SaveChangesAsync();

        var result = await _sut.GetPagedProductsAsync(null, "price_asc", null, false, 1, 10, []);

        var ids = result.Items.Select(p => p.Id).ToList();
        ids.IndexOf(cheap.Id).Should().BeLessThan(ids.IndexOf(expensive.Id));
    }

    [Fact]
    public async Task GetPagedProductsAsync_Overload_ExcludesDeletedProductsAndFiltersByPriceRange()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var deleted = TestEntityFactory.CreateProduct(category, user, isDeleted: true);
        var expensive = TestEntityFactory.CreateProduct(category, user, price: 5000m);
        var affordable = TestEntityFactory.CreateProduct(category, user, price: 1000m);
        Context.Products.AddRange(deleted, expensive, affordable);
        AddReview(deleted);
        AddReview(expensive);
        AddReview(affordable);
        await Context.SaveChangesAsync();

        var result = await _sut.GetPagedProductsAsync(null, 1, 10, [], [], [], 2000);

        var ids = result.Items.Select(p => p.Id).ToList();
        ids.Should().Contain(affordable.Id);
        ids.Should().NotContain(deleted.Id);
        ids.Should().NotContain(expensive.Id);
    }

    [Fact]
    public async Task GetPagedProductsAsync_Overload_FiltersByFlowerIds()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var flower = TestEntityFactory.CreateFlower(color: "Šampanj");
        var matching = TestEntityFactory.CreateProduct(category, user);
        var nonMatching = TestEntityFactory.CreateProduct(category, user);
        Context.Flowers.Add(flower);
        Context.Products.AddRange(matching, nonMatching);
        Context.ProductFlowers.Add(TestEntityFactory.CreateProductFlower(matching, flower));
        AddReview(matching);
        AddReview(nonMatching);
        await Context.SaveChangesAsync();

        var result = await _sut.GetPagedProductsAsync(null, 1, 10, [], [], [flower.Id], 0);

        var ids = result.Items.Select(p => p.Id).ToList();
        ids.Should().Contain(matching.Id);
        ids.Should().NotContain(nonMatching.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProductWithRelatedData()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user);
        Context.Products.Add(product);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var result = await _sut.GetByIdAsync(product.Id);

        result.Should().NotBeNull();
        result!.Category.Should().NotBeNull();
        result.User.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(-1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetProductsByIdsAsync_ReturnsMatchingProducts()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var productOne = TestEntityFactory.CreateProduct(category, user);
        var productTwo = TestEntityFactory.CreateProduct(category, user);
        Context.Products.AddRange(productOne, productTwo);
        await Context.SaveChangesAsync();

        var result = await _sut.GetProductsByIdsAsync([productOne.Id, productTwo.Id]);

        result.Select(p => p.Id).Should().BeEquivalentTo([productOne.Id, productTwo.Id]);
    }

    [Fact]
    public async Task ExistsAsync_ById_WhenProductExists_ReturnsTrue()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user);
        Context.Products.Add(product);
        await Context.SaveChangesAsync();

        var result = await _sut.ExistsAsync(product.Id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ById_WhenProductDoesNotExist_ReturnsFalse()
    {
        var result = await _sut.ExistsAsync(-1);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_ByIds_WhenAnyProductMatches_ReturnsTrue()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user);
        Context.Products.Add(product);
        await Context.SaveChangesAsync();

        var result = await _sut.ExistsAsync([product.Id, -1]);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ByIds_WhenNoneMatch_ReturnsFalse()
    {
        var result = await _sut.ExistsAsync([-1, -2]);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByNameAsync_WhenProductWithNameExists_ReturnsTrue()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user, name: "Jedinstveni naziv");
        Context.Products.Add(product);
        await Context.SaveChangesAsync();

        var result = await _sut.ExistsByNameAsync("Jedinstveni naziv");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByNameAsync_WhenNoProductWithNameExists_ReturnsFalse()
    {
        var result = await _sut.ExistsByNameAsync("Nepostojeći naziv");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CountAvailableProductsAsync_CountsOnlyNonDeletedProducts()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var active = TestEntityFactory.CreateProduct(category, user, isDeleted: false);
        var deleted = TestEntityFactory.CreateProduct(category, user, isDeleted: true);
        Context.Products.AddRange(active, deleted);
        await Context.SaveChangesAsync();

        var result = await _sut.CountAvailableProductsAsync();

        result.Should().Be(1);
    }
}
