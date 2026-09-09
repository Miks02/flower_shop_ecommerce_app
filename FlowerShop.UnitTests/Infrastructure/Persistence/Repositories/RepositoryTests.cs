using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.UnitTests.Infrastructure.Persistence.Repositories;

public class RepositoryTests : RepositoryTestBase
{
    private readonly CategoryRepository _sut;

    public RepositoryTests()
    {
        _sut = new CategoryRepository(Context);
    }

    [Fact]
    public async Task Add_AddsEntityToContext()
    {
        var category = TestEntityFactory.CreateCategory();

        _sut.Add(category);
        await Context.SaveChangesAsync();

        var stored = await Context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == category.Id);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be(category.Name);
    }

    [Fact]
    public async Task Update_MarksEntityAsModified()
    {
        var category = TestEntityFactory.CreateCategory();
        Context.Categories.Add(category);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var tracked = await Context.Categories.FirstAsync(c => c.Id == category.Id);
        tracked.Name = "Izmenjeno ime";

        _sut.Update(tracked);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var stored = await Context.Categories.AsNoTracking().FirstAsync(c => c.Id == category.Id);
        stored.Name.Should().Be("Izmenjeno ime");
    }

    [Fact]
    public async Task Remove_DeletesEntityFromContext()
    {
        var category = TestEntityFactory.CreateCategory();
        Context.Categories.Add(category);
        await Context.SaveChangesAsync();

        _sut.Remove(category);
        await Context.SaveChangesAsync();

        var stored = await Context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == category.Id);
        stored.Should().BeNull();
    }
}
