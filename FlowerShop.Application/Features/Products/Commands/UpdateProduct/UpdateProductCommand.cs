using FlowerShop.Domain.Entities.Products;
using Microsoft.AspNetCore.Http;

namespace FlowerShop.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CategoryId { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? PromoPrice { get; set; }
    public DiscountType DiscountType { get; set; } = DiscountType.None;
    public int Stock { get; set; }
    public IFormFile? ProductImage { get; set; }
    public IReadOnlyList<FlowerItemDto> Flowers { get; set; } = [];
    public IReadOnlyList<int> Occasions { get; set; } = [];
}