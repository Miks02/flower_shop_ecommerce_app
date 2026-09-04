using FlowerShop.Domain.Entities.Products;
using FlowerShop.Domain.Enums;

namespace FlowerShop.Application.Features.Catalogue.Queries.GetProductDetails;

public record GetProductDetailsResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string ImageUrl { get; init; } = null!;
    public decimal Price { get; init; }
    public decimal? PromoPrice { get; init; }
    public bool IsOnPromotion { get; init; }
    public DiscountType DiscountType { get; init; }
    public int Stock { get; init; }
    public string CategoryName { get; init; } = null!;
    public IReadOnlyList<string> Occasions { get; init; } = [];
    public IReadOnlyList<FlowerCompositionDto> Composition { get; init; } = [];
}

public record FlowerCompositionDto
{
    public string Name { get; init; } = null!;
    public string Color { get; init; } = null!;
    public int Quantity { get; init; }
    public FlowerCategory Category { get; init; }
}
