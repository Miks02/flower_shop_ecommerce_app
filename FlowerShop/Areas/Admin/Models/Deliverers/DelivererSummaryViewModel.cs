using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Web.Areas.Admin.Models.Deliverers;

public record DelivererSummaryViewModel
{
    public PagedResult<DelivererListViewModel> PagedDeliverers { get; init; } = null!;
    public int TotalCount { get; init; }
    public int AvailableCount { get; init; }
    public int OnDutyCount { get; init; }
    public int UnavailableCount { get; init; }
    public int BicycleCount { get; init; }
    public int ScooterCount { get; init; }
    public int CarCount { get; init; }
}