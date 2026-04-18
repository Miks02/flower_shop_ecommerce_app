namespace FlowerShop.Domain.Entities.Flowers;

public interface IFlowerRepository
{
    Task<IReadOnlyList<int>> GetInvalidFlowerIdsAsync(IReadOnlyList<int> flowerIds, CancellationToken ct = default);
    void Add(Flower flower);
    void Update(Flower flower);
    void Remove(Flower flower);
    Task SaveChangesAsync(CancellationToken ct = default);
}