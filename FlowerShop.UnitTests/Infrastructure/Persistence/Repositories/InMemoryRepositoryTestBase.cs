using FlowerShop.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.UnitTests.Infrastructure.Persistence.Repositories;

public abstract class InMemoryRepositoryTestBase : IDisposable
{
    protected readonly AppDbContext Context;

    protected InMemoryRepositoryTestBase()
    {
        Context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        GC.SuppressFinalize(this);
    }
}
