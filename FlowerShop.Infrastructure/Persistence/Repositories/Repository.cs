using FlowerShop.Infrastructure.Persistence.EntityFramework;

namespace FlowerShop.Infrastructure.Persistence.Repositories;

public abstract class Repository<TEntity>(AppDbContext context)
    where TEntity : class
{
    protected AppDbContext Context { get; } = context;

    public void Add(TEntity entity)
    {
        Context.Set<TEntity>().Add(entity);
    }

    public void Update(TEntity entity)
    {
        Context.Set<TEntity>().Update(entity);
    }

    public void Remove(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
    }

}