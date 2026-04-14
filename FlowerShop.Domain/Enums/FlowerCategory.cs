using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain.Enums;

public enum FlowerCategory
{
    [Display(Name = "Sveže")]
    Fresh,
    [Display(Name = "Dehidrirano")]
    Dehydrated,
    [Display(Name = "Veštačko")]
    Artificial
}