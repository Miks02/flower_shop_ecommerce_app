using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Web.Enums;

public enum FlowerCategory
{
    [Display(Name = "Sveže")]
    Fresh,
    [Display(Name = "Dehidrirano")]
    Dehydrated,
    [Display(Name = "Veštačko")]
    Artificial
}