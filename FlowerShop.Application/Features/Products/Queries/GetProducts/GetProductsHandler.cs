using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Products.Queries.GetProducts;

public class GetProductsHandler(IProductRepository productRepo) : IHandler
{
    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct = default)
    {
        return await productRepo.GetPagedProductsAsync(
            request.Name,
            request.SortBy,
            request.CategoryId,
            request.IsDeleted,
            request.PageIndex,
            request.PageSize,
            request.OccasionIds, ct);
    }
}