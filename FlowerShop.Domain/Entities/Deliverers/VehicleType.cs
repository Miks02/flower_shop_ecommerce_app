using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain.Entities.Deliverers;

public enum VehicleType
{
    [Display(Name = "Bicikla")]
    Bicycle = 0,
    [Display(Name = "Skuter")]
    Scooter = 1,
    [Display(Name = "Automobil")]
    Car = 2
}