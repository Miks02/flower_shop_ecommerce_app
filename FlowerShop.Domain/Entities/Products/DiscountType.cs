using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain.Entities.Products;

public enum DiscountType
{
    None = 0,
    [Display(Name = "Procenat popusta")]
    PercentageOff = 1,
    [Display(Name = "Fiksni iznos")]
    FixedAmount = 2,
    [Display(Name = "Rasprodaja")]
    Clearance = 3,
    [Display(Name = "Sezonska akcija")]
    SeasonalSale = 4,
    [Display(Name = "Munjevita akcija")]
    FlashSale = 5,
    [Display(Name = "Promotivna akcija")]
    Promotional = 6,

    [Display(Name = "Dan žena")]
    WomensDay = 10,
    [Display(Name = "Dan zaljubljenih")]
    ValentinesDay = 11,
    [Display(Name = "Dan majki")]
    MothersDay = 12,
    [Display(Name = "Dan nastavnika")]
    TeachersDay = 13,
    [Display(Name = "Matura")]
    Graduation = 14,
    [Display(Name = "Nova godina")]
    NewYear = 15,
    [Display(Name = "Božić")]
    Christmas = 16
}