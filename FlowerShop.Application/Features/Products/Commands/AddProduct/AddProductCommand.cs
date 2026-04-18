using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Entities.Ocassions;
using Microsoft.AspNetCore.Http;

namespace FlowerShop.Application.Features.Products.Commands.AddProduct;

public record AddProductCommand
{
    public string UserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int CategoryId { get; set; }
    public int OccasionId { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public IFormFile ProductImage { get; set; } = null!;

    public IReadOnlyList<FlowerItemDto> Flowers { get; set; } = [];
    public IReadOnlyList<int> Occasions { get; set; } = [];

}