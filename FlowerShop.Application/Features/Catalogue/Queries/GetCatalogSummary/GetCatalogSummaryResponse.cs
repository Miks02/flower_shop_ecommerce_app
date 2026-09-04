using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Catalogue.Queries.GetCatalogSummary;

public record GetCatalogSummaryResponse
{
    public PagedResult<ProductDto> PagedProducts { get; init; } = null!;
    public IReadOnlyList<CategoryDto> Categories { get; init; } = [];
    public IReadOnlyList<OccasionDto> Occasions { get; init; } = [];
}
