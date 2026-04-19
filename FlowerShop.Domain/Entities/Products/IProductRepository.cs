using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Products;

public interface IProductRepository
{
    Task<PagedResult<ProductDto>> GetPagedProductsAsync(
        string? search,
        string? sortBy,
        int? categoryId,
        bool isDeleted,
        int page,
        int pageSize,
        IReadOnlyList<int> occasionIds,
        CancellationToken ct = default);
    void Add(Product product);
    void Update(Product product);
    void Remove(Product product);
}