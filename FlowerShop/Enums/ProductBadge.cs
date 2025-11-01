using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Enums;

public enum ProductBadge
{
    [Display(Name = "AKCIJA")]
    Sale,
    [Display(Name = "NOVO")]   
    New,
    [Display(Name = "NEMA NA STANJU")]   
    OutOfStock,

}