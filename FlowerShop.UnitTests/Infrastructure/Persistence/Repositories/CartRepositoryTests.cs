using FlowerShop.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.UnitTests.Infrastructure.Persistence.Repositories;

public class CartRepositoryTests : RepositoryTestBase
{
    private readonly CartRepository _sut;

    public CartRepositoryTests()
    {
        _sut = new CartRepository(Context);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenCartExists_ReturnsCartWithItemsAndProducts()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user);
        var cart = TestEntityFactory.CreateCart(user);
        var item = TestEntityFactory.CreateCartItem(cart, product, quantity: 2);
        Context.Products.Add(product);
        Context.Carts.Add(cart);
        Context.CartItems.Add(item);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var result = await _sut.GetByUserIdAsync(user.Id);

        result.Should().NotBeNull();
        result!.Items.Should().ContainSingle();
        result.Items.First().Product.Should().NotBeNull();
        result.Items.First().Product.Id.Should().Be(product.Id);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenCartDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByUserIdAsync("unknown-user");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetItemByIdForUserAsync_WhenItemBelongsToUser_ReturnsItem()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user);
        var cart = TestEntityFactory.CreateCart(user);
        var item = TestEntityFactory.CreateCartItem(cart, product);
        Context.Products.Add(product);
        Context.Carts.Add(cart);
        Context.CartItems.Add(item);
        await Context.SaveChangesAsync();

        var result = await _sut.GetItemByIdForUserAsync(item.Id, user.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(item.Id);
    }

    [Fact]
    public async Task GetItemByIdForUserAsync_WhenItemBelongsToAnotherUser_ReturnsNull()
    {
        var owner = TestEntityFactory.CreateUser();
        var otherUser = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, owner);
        var cart = TestEntityFactory.CreateCart(owner);
        var item = TestEntityFactory.CreateCartItem(cart, product);
        Context.Products.Add(product);
        Context.Carts.Add(cart);
        Context.CartItems.Add(item);
        await Context.SaveChangesAsync();

        var result = await _sut.GetItemByIdForUserAsync(item.Id, otherUser.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task RemoveItem_RemovesCartItem()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user);
        var cart = TestEntityFactory.CreateCart(user);
        var item = TestEntityFactory.CreateCartItem(cart, product);
        Context.Products.Add(product);
        Context.Carts.Add(cart);
        Context.CartItems.Add(item);
        await Context.SaveChangesAsync();

        _sut.RemoveItem(item);
        await Context.SaveChangesAsync();

        var stored = await Context.CartItems.AsNoTracking().FirstOrDefaultAsync(i => i.Id == item.Id);
        stored.Should().BeNull();
    }

    [Fact]
    public async Task CountItemsAsync_ReturnsNumberOfItemsInUsersCart()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var productOne = TestEntityFactory.CreateProduct(category, user);
        var productTwo = TestEntityFactory.CreateProduct(category, user);
        var cart = TestEntityFactory.CreateCart(user);
        Context.Products.AddRange(productOne, productTwo);
        Context.Carts.Add(cart);
        Context.CartItems.AddRange(
            TestEntityFactory.CreateCartItem(cart, productOne),
            TestEntityFactory.CreateCartItem(cart, productTwo));
        await Context.SaveChangesAsync();

        var result = await _sut.CountItemsAsync(user.Id);

        result.Should().Be(2);
    }

    [Fact]
    public async Task CountItemsAsync_WhenUserHasNoCart_ReturnsZero()
    {
        var result = await _sut.CountItemsAsync("unknown-user");

        result.Should().Be(0);
    }

    [Fact]
    public async Task ProductExistsInAnyCartAsync_WhenProductIsInACart_ReturnsTrue()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user);
        var cart = TestEntityFactory.CreateCart(user);
        Context.Products.Add(product);
        Context.Carts.Add(cart);
        Context.CartItems.Add(TestEntityFactory.CreateCartItem(cart, product));
        await Context.SaveChangesAsync();

        var result = await _sut.ProductExistsInAnyCartAsync(product.Id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ProductExistsInAnyCartAsync_WhenProductIsNotInAnyCart_ReturnsFalse()
    {
        var user = TestEntityFactory.CreateUser();
        var category = TestEntityFactory.CreateCategory();
        var product = TestEntityFactory.CreateProduct(category, user);
        Context.Products.Add(product);
        await Context.SaveChangesAsync();

        var result = await _sut.ProductExistsInAnyCartAsync(product.Id);

        result.Should().BeFalse();
    }
}
