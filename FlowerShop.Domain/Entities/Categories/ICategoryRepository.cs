namespace FlowerShop.Domain.Entities.Categories;

public interface ICategoryRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    void Add(Category category);
    void Update(Category category);
    void Remove(Category category);
}