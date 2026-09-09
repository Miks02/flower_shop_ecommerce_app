using FlowerShop.Domain.Enums;

namespace FlowerShop.Web.ViewModels.Components;

public record ProductCardViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
    public decimal Price { get; init; }
    public decimal PromoPrice { get; init; }
    public decimal? Rating { get; init; }
    public string Category { get; init; } = null!;
    public string BadgeColor { get; init; } = string.Empty;
    public ProductBadge? Badge { get; init; }
    public bool IsAvailable { get; init; }
    public bool IsNew { get; init; }
    public bool IsDiscounted => PromoPrice > 0;
}