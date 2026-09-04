using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Catalogue.Queries.GetCatalog;
using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Ocassions;

namespace FlowerShop.Application.Features.Catalogue.Queries.GetCatalogSummary;

public class GetCatalogSummaryHandler(
    GetCatalogHandler getCatalogHandler,
    ICategoryRepository categoryRepo,
    IOccasionRepository occasionRepo) : IHandler
{
    public async Task<GetCatalogSummaryResponse> Handle(GetCatalogSummaryQuery request, CancellationToken ct = default)
    {
        var productRequest = new GetCatalogQuery
        {
            PriceRange = request.PriceRange,
            OccasionIds = request.OccasionIds,
            CategoryIds = request.CategoryIds,
            Page = request.Page,
            PageSize = request.PageSize,
            Sort = request.Sort
        };

        return new GetCatalogSummaryResponse
        {
            PagedProducts = await getCatalogHandler.Handle(productRequest, ct),
            Categories = await categoryRepo.GetAllAsync(ct),
            Occasions = await occasionRepo.GetAllAsync(ct)
        };
    }
}
