using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Infrastructure.Persistence.EntityFramework;

namespace FlowerShop.Infrastructure.Persistence.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task<int> SaveAsync(CancellationToken ct) 
        => await context.SaveChangesAsync(ct);
}