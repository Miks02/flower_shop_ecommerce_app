using FlowerShop.Application.Features.Catalogue.Queries.GetProductDetails;
using FlowerShop.Domain.Entities.Products;

namespace FlowerShop.Web.ViewModels;

public record ProductDetailsViewModel
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
    public bool IsAvailable => Stock > 0;
}
