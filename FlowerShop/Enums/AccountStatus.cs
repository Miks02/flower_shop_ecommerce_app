using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Web.Enums;

public enum AccountStatus
{
    [Display(Name = "Aktivan")]
    Active,
    [Display(Name = "Neaktivan")]
    Inactive
}