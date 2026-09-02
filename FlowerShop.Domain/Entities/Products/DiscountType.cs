using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain.Entities.Products;

public enum DiscountType
{
    None = 0,
    [Display(Name = "Rasprodaja")] 
    Clearance = 1,
    [Display(Name = "Sezonska akcija")] 
    SeasonalSale = 2,
    [Display(Name = "Munjevita akcija")] 
    FlashSale = 3,
    [Display(Name = "Promotivna akcija")] 
    Promotional = 4,

    [Display(Name = "Dan žena")]
    WomensDay = 10,
    [Display(Name = "Dan zaljubljenih")] 
    ValentinesDay = 11,
    [Display(Name = "Dan majki")] 
    MothersDay = 12,
    [Display(Name = "Matura / Diplomski")] 
    Graduation = 13,
    [Display(Name = "Nova godina")] 
    NewYear = 14,
    [Display(Name = "Božić")] 
    Christmas = 15
}