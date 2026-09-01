
using Microsoft.AspNetCore.Http;

namespace FlowerShop.Application.Features.Products.Commands.AddProduct;

public record AddProductCommand
{
    public string UserId { get; init; } = null!;
    public string Name { get; init; } = null!;
    public int CategoryId { get; init; }
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }

    public IFormFile ProductImage { get; init; } = null!;

    public IReadOnlyList<FlowerItemDto> Flowers { get; init; } = [];
    public IReadOnlyList<int> Occasions { get; init; } = [];

}