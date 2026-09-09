using FlowerShop.Domain.Entities.Products;

namespace FlowerShop.Web.Areas.Admin.Models.Products;

public record ProductListViewModel
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public string Description { get; init; } = string.Empty;
    public string ProductImage { get; init; } = null!;
    public decimal Price { get; init; }
    public decimal? PromoPrice { get; init; }
    public DiscountType DiscountType { get; init; } = DiscountType.None;
    public int Stock { get; init; }
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; init; }
    public IReadOnlyList<string> Occasions { get; init; } = [];
    public IReadOnlyList<string> FlowerNames { get; init; } = [];

    public bool IsDiscounted => DiscountType != DiscountType.None && PromoPrice.HasValue;
}