using FlowerShop.Domain.Entities.Deliverers;

namespace FlowerShop.Application.Features.Deliverers.Queries.GetDeliverersSummary;

public record GetDeliverersSummaryQuery
{
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public VehicleType? VehicleType { get; set; }
    public DelivererStatus? DelivererStatus { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
