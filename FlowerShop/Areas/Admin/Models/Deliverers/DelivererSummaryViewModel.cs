using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Web.Areas.Admin.Models.Deliverers;

public record DelivererSummaryViewModel
{
    public PagedResult<DelivererListViewModel> PagedDeliverers { get; set; } = null!;
    public int TotalCount { get; set; }
    public int AvailableCount { get; set; }
    public int OnDutyCount { get; set; }
    public int UnavailableCount { get; set; }
    public int BicycleCount { get; set; }
    public int ScooterCount { get; set; }
    public int CarCount { get; set; }
}