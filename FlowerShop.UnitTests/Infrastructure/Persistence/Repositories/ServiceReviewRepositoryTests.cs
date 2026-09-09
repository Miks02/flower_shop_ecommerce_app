using FlowerShop.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.UnitTests.Infrastructure.Persistence.Repositories;

public class ServiceReviewRepositoryTests : RepositoryTestBase
{
    private readonly ServiceReviewRepository _sut;

    public ServiceReviewRepositoryTests()
    {
        _sut = new ServiceReviewRepository(Context);
    }

    [Fact]
    public async Task Add_PersistsServiceReview()
    {
        var user = TestEntityFactory.CreateUser();
        var order = TestEntityFactory.CreateOrder(user);
        Context.Users.Add(user);
        Context.Orders.Add(order);
        await Context.SaveChangesAsync();

        var review = TestEntityFactory.CreateServiceReview(order, user, rating: 4, comment: "Odlična usluga");

        _sut.Add(review);
        await Context.SaveChangesAsync();

        var stored = await Context.ServiceReviews.AsNoTracking().FirstOrDefaultAsync(r => r.OrderId == order.Id);
        stored.Should().NotBeNull();
        stored!.Rating.Should().Be(4);
        stored.Comment.Should().Be("Odlična usluga");
    }

    [Fact]
    public async Task Remove_DeletesServiceReview()
    {
        var user = TestEntityFactory.CreateUser();
        var order = TestEntityFactory.CreateOrder(user);
        var review = TestEntityFactory.CreateServiceReview(order, user);
        Context.Users.Add(user);
        Context.Orders.Add(order);
        Context.ServiceReviews.Add(review);
        await Context.SaveChangesAsync();

        _sut.Remove(review);
        await Context.SaveChangesAsync();

        var stored = await Context.ServiceReviews.AsNoTracking().FirstOrDefaultAsync(r => r.Id == review.Id);
        stored.Should().BeNull();
    }
}
