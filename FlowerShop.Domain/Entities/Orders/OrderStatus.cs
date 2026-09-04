using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain.Entities.Orders;

public enum OrderStatus
{
    [Display(Name = "Na čekanju")]
    Pending,
    [Display(Name = "Potvrđeno")]
    Confirmed,
    [Display(Name = "Završeno")]
    Completed,
    [Display(Name = "Otkazano")]
    Cancelled
}