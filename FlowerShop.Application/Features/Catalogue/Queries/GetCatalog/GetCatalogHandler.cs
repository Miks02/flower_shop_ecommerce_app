using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Catalogue.Queries.GetCatalog;

public class GetCatalogHandler(IProductRepository productRepo) : IHandler
{
    public async Task<PagedResult<ProductDto>> Handle(GetCatalogQuery request, CancellationToken ct = default)
    {
        var products = await productRepo.GetPagedProductsAsync(
            request.Sort,
            request.Page, 
            request.PageSize,
            request.CategoryIds,
            request.OccasionIds,
            request.PriceRange, ct);
        
        return products;
    }
}