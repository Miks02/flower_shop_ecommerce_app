using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Domain.Entities.Products;

namespace FlowerShop.Application.Features.Products.Queries.GetProductsSummary;

public class GetProductsSummaryHandler(
    IProductRepository productRepo, 
    IOccasionRepository occasionRepo, 
    ICategoryRepository categoryRepo) : IHandler
{
    public async Task<GetProductsSummaryResponse> Handle(
        GetProductsSummaryQuery request,
        CancellationToken ct = default)
    {
        return new GetProductsSummaryResponse
        {
            PagedProducts = await productRepo.GetPagedProductsAsync(
                request.SearchBy,
                request.SortBy,
                request.CategoryId,
                request.IsDeleted,
                request.PageIndex,
                request.PageSize,
                request.OccasionIds, ct),
            Occasions = await occasionRepo.GetAllAsync(ct),
            Categories = await categoryRepo.GetAllAsync(ct)
        };
    }
}