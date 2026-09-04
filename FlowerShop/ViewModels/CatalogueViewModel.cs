using FlowerShop.Application.Features.Catalogue.Queries;
using FlowerShop.Application.Features.Catalogue.Queries.GetCatalog;
using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Web.ViewModels;

public class CatalogueViewModel
{
    public PagedResult<ProductDto> PagedProducts { get; set; } = null!;
    public IReadOnlyList<CategoryDto> Categories { get; set; } = [];
    public IReadOnlyList<OccasionDto> Occasions { get; set; } = [];
    public List<int> CategoryIds { get; set; } = [];
    public List<int> OccasionIds { get; set; } = [];
    public int PriceRange { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    public string Sort { get; set; } = "name_asc";

    public GetCatalogQuery ToQueryRequest()
    {
        return new GetCatalogQuery
        {
            PriceRange = PriceRange,
            OccasionIds = OccasionIds.ToList(),
            CategoryIds = CategoryIds.ToList(),
            Page = Page,
            PageSize = PageSize,
            Sort = Sort
        };
    }
}