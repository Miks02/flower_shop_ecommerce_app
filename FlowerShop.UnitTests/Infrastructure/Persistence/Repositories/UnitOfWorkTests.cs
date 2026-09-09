using FlowerShop.Infrastructure.Persistence.Repositories;
using FluentAssertions;

namespace FlowerShop.UnitTests.Infrastructure.Persistence.Repositories;

public class UnitOfWorkTests : RepositoryTestBase
{
    private readonly UnitOfWork _sut;

    public UnitOfWorkTests()
    {
        _sut = new UnitOfWork(Context);
    }

    [Fact]
    public async Task SaveAsync_PersistsPendingChangesAndReturnsAffectedRowCount()
    {
        var category = TestEntityFactory.CreateCategory();
        Context.Categories.Add(category);

        var result = await _sut.SaveAsync(CancellationToken.None);

        result.Should().Be(1);
        Context.Categories.Local.Should().Contain(category);
    }

    [Fact]
    public async Task CommitAsync_WithoutStartedTransaction_ThrowsInvalidOperationException()
    {
        var act = async () => await _sut.CommitAsync();

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task RollbackAsync_WithoutStartedTransaction_DoesNotThrow()
    {
        var act = async () => await _sut.RollbackAsync();

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task BeginTransactionAsync_ThenCommitAsync_PersistsChanges()
    {
        await _sut.BeginTransactionAsync();
        var category = TestEntityFactory.CreateCategory();
        Context.Categories.Add(category);
        await Context.SaveChangesAsync();

        await _sut.CommitAsync();

        var result = Context.Categories.Any(c => c.Id == category.Id);
        result.Should().BeTrue();
    }
}
