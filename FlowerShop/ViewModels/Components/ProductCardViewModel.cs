using FlowerShop.Enums;

namespace FlowerShop.ViewModels.Components;

public class ProductCardViewModel
{
    public string Name { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal PromoPrice { get; set; }
    public double Rating { get; set; } 
    public string Category { get; set; } = null!;
    public string BadgeColor { get; set; } = string.Empty;
    public ProductBadge? Badge { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsNew { get; set; }
    public bool IsDiscounted => PromoPrice > 0;
    
    

}