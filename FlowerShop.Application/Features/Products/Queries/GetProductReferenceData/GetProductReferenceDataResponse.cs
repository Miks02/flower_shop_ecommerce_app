using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Ocassions;

namespace FlowerShop.Application.Features.Products.Queries.GetProductReferenceData;

public record GetProductReferenceDataResponse
{
    public IReadOnlyList<OccasionDto> Occasions { get; init; } = [];
    public IReadOnlyList<CategoryDto> Categories { get; init; } = [];
};