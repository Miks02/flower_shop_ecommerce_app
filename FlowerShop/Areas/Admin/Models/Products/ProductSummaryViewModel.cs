using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Web.Areas.Admin.Models.Products;

public class ProductSummaryViewModel
{
    public PagedResult<ProductListViewModel> PagedProducts { get; set; } = null!;
    public IReadOnlyList<CategoryDto> Categories { get; set; } = [];
    public IReadOnlyList<OccasionDto> Occasions { get; set; } = [];
}