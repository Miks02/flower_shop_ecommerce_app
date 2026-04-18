namespace FlowerShop.Domain.Entities.Ocassions;

public interface IOccasionRepository
{
    Task<IReadOnlyList<int>> GetInvalidOccasionIdsAsync(IReadOnlyList<int> occasionIds, CancellationToken ct = default);
    Task<IReadOnlyList<Occasion>> GetOccasionsByIdsAsync(IReadOnlyList<int> occasionIds, CancellationToken ct = default);
    void Add(Occasion occasion);
    void Update(Occasion occasion);
    void Remove(Occasion occasion);
    Task SaveChangesAsync(CancellationToken ct = default);
}