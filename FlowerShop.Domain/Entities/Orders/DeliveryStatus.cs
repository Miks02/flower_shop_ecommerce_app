using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain.Entities.Orders;

public enum DeliveryStatus
{
    [Display(Name = "Na čekanju")]
    Standby,
    [Display(Name = "Pripremljeno")]
    Prepared,
    [Display(Name = "Na putu")]
    InTransit,
    [Display(Name = "Na putu")]
    OnTheWay,
    [Display(Name = "Dostavljeno")]
    Delivered
}