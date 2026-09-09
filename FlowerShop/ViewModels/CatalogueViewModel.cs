using FlowerShop.Application.Features.Catalogue.Queries;
using FlowerShop.Application.Features.Catalogue.Queries.GetCatalog;
using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Web.ViewModels;

public record CatalogueViewModel
{
    public PagedResult<ProductDto> PagedProducts { get; init; } = null!;
    public IReadOnlyList<CategoryDto> Categories { get; init; } = [];
    public IReadOnlyList<OccasionDto> Occasions { get; init; } = [];
    public IReadOnlyList<CatalogueFlowerDto> Flowers { get; init; } = [];
    public List<int> CategoryIds { get; init; } = [];
    public List<int> OccasionIds { get; init; } = [];
    public List<int> FlowerIds { get; init; } = [];
    public int PriceRange { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 15;
    public string Sort { get; init; } = "name_asc";

    public GetCatalogQuery ToQueryRequest()
    {
        return new GetCatalogQuery
        {
            PriceRange = PriceRange,
            OccasionIds = OccasionIds.ToList(),
            CategoryIds = CategoryIds.ToList(),
            FlowerIds = FlowerIds.ToList(),
            Page = Page,
            PageSize = PageSize,
            Sort = Sort
        };
    }
}