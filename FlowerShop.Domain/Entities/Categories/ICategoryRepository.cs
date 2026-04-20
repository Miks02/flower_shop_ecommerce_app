namespace FlowerShop.Domain.Entities.Categories;

public interface ICategoryRepository
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<Category?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    void Add(Category category);
    void Update(Category category);
    void Remove(Category category);
}