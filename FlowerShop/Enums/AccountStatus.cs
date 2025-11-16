using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Enums;

public enum AccountStatus
{
    [Display(Name = "Aktivan")]
    Active,
    [Display(Name = "Neaktivan")]
    Inactive
}