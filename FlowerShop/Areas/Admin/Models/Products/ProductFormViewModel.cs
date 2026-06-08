using FlowerShop.Application.Features.Products.Commands.AddProduct;
using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Entities.Ocassions;

namespace FlowerShop.Web.Areas.Admin.Models.Products;

public record ProductFormViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IFormFile? ProductImage { get; set; }
    public string? ProductImageUrl { get; set; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public int CategoryId { get; init; }
    public List<int> OccasionIds { get; init; } = [];
    public List<FlowerItemDto> SelectedFlowers { get; init; } = [];
    public string CreatedByName { get; init; } = string.Empty;
    public string CreatedById { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; init; }
    
    public IReadOnlyList<OccasionDto> AvailableOccasions { get; init; } = [];
    public IReadOnlyList<CategoryDto> AvailableCategories { get; init; } = [];
    public IReadOnlyList<FlowerDto> AvailableFlowers { get; init; } = [];
    
    public bool OutOfStock => Stock <= 0;

};