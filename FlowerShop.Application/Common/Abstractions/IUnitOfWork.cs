namespace FlowerShop.Application.Common.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveAsync(CancellationToken ct = default);
}