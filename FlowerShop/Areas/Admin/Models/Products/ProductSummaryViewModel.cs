using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Web.Areas.Admin.Models.Products;

public record ProductSummaryViewModel
{
    public PagedResult<ProductListViewModel> PagedProducts { get; init; } = null!;
    public IReadOnlyList<CategoryDto> Categories { get; init; } = [];
    public IReadOnlyList<OccasionDto> Occasions { get; init; } = [];
}