using FlowerShop.Domain.Enums;

namespace FlowerShop.Domain.Entities.Flowers;

public interface IFlowerRepository
{
    Task<IReadOnlyList<FlowerDto>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Flower>> GetFlowersByIdsAsync(IReadOnlyList<int> flowerIds, CancellationToken ct = default);
    Task<IReadOnlyList<int>> GetInvalidFlowerIdsAsync(IReadOnlyList<int> flowerIds, CancellationToken ct = default);
    Task<Flower?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(string name, string color, FlowerCategory flowerCategory, CancellationToken ct = default);
    Task<bool> IsUsedInProductsAsync(int id, CancellationToken ct = default);
    void Add(Flower flower);
    void Update(Flower flower);
    void Remove(Flower flower);
}