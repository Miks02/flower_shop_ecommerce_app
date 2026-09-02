using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Deliverers.Queries.GetDeliverersSummary;

public record GetDeliverersSummaryResponse
{
    public PagedResult<DelivererDto> PagedDeliverers { get; set; } = null!;
    public DelivererStatisticsDto Statistics { get; set; } = null!;
}
