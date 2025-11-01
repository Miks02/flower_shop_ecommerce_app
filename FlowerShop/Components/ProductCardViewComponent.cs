using FlowerShop.Enums;
using FlowerShop.ViewModels.Components;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Components;

public class ProductCardViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync
        (
            string name, 
            string imageUrl, 
            decimal price, 
            decimal promoPrice, 
            double rating, 
            string category,
            bool isAvailable,
            bool isDiscounted,
            bool isNew
        )
    {
        ProductBadge? badge = GetBadge(isAvailable, isNew, isDiscounted);
        string badgeColor = string.Empty;
        
        
        if (badge is not null)
        {
            switch (badge.Value)
            {
                case ProductBadge.New:
                    badgeColor = "badge new";
                    break;
                case ProductBadge.Sale:
                    badgeColor = "badge sale";
                    break;
                case ProductBadge.OutOfStock:
                    badgeColor = "badge out-of-stock";
                    break;
            }
        }

        var vm = new ProductCardViewModel()
        {
            Name = name,
            ImageUrl = imageUrl,
            Price = price,
            PromoPrice = promoPrice,
            Rating = rating,
            Category = category,
            BadgeColor = badgeColor,
            Badge = badge,

        };
        
        return await Task.FromResult(View("Default", vm));       
    }
    
    public ProductBadge? GetBadge(bool isAvailable, bool isNew, bool isDiscounted)
    {
        if (isAvailable is false)
            return ProductBadge.OutOfStock;
        if(isDiscounted)
            return ProductBadge.Sale;
        if(isNew)
            return ProductBadge.New;
        return null;       
    }
}