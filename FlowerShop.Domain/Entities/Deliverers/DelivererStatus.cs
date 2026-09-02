using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain.Entities.Deliverers;

public enum DelivererStatus
{
    [Display(Name = "Dostupan")]
    Available,
    [Display(Name = "Na dostavi")]
    OnDuty,
    [Display(Name = "Nedostupan")]
    Unavailable
}