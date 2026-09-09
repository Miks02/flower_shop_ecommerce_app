using FlowerShop.Infrastructure.Persistence.Repositories;
using FluentAssertions;

namespace FlowerShop.UnitTests.Infrastructure.Persistence.Repositories;

public class ProductReviewRepositoryTests : RepositoryTestBase
{
    private readonly ProductReviewRepository _sut;

    public ProductReviewRepositoryTests()
    {
        _sut = new ProductReviewRepository(Context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenReviewExists_ReturnsReviewWithProductAndUser()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user);
        var review = TestEntityFactory.CreateProductReview(product, user, rating: 5, comment: "Predivno");
        Context.Products.Add(product);
        Context.ProductReviews.Add(review);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var result = await _sut.GetByIdAsync(review.Id);

        result.Should().NotBeNull();
        result!.Comment.Should().Be("Predivno");
        result.Product.Should().NotBeNull();
        result.Product.Id.Should().Be(product.Id);
        result.User.Should().NotBeNull();
        result.User.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenReviewDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(-1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByProductAndReviewerAsync_WhenReviewExists_ReturnsReview()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user);
        var review = TestEntityFactory.CreateProductReview(product, user);
        Context.Products.Add(product);
        Context.ProductReviews.Add(review);
        await Context.SaveChangesAsync();

        var result = await _sut.GetByProductAndReviewerAsync(product.Id, user.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(review.Id);
    }

    [Fact]
    public async Task GetByProductAndReviewerAsync_WhenReviewDoesNotExist_ReturnsNull()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user);
        Context.Products.Add(product);
        await Context.SaveChangesAsync();

        var result = await _sut.GetByProductAndReviewerAsync(product.Id, user.Id);

        result.Should().BeNull();
    }
}
