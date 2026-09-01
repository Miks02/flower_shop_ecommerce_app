using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Products.Queries.GetProductsSummary;

public record GetProductsSummaryResponse
{
    public PagedResult<ProductDto> PagedProducts { get; set; }
    public IReadOnlyList<CategoryDto> Categories { get; set; }
    public IReadOnlyList<OccasionDto> Occasions { get; set; }
}