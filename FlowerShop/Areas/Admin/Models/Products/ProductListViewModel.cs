namespace FlowerShop.Web.Areas.Admin.Models.Products;

public record ProductListViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ProductImage { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public IReadOnlyList<string> Occasions { get; set; } = [];
    public IReadOnlyList<string> FlowerNames { get; set; } = [];
}