namespace FlowerShop.Application.Common.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAksync(CancellationToken ct = default);
}