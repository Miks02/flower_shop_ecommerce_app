using FlowerShop.Infrastructure.Persistence.EntityFramework;

namespace FlowerShop.Infrastructure.Persistence.Repositories;

public abstract class Repository<TEntity>(AppDbContext context) 
    where TEntity : class
{

    public void Add(TEntity entity)
    {
        context.Set<TEntity>().Add(entity);
    }
    
    public void Update(TEntity entity)
    {
        context.Set<TEntity>().Update(entity);
    }
    
    public void Remove(TEntity entity)
    {
        context.Set<TEntity>().Remove(entity);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await context.SaveChangesAsync(ct);
    }
    
}