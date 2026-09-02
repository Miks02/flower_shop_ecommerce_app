using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Deliverers;

namespace FlowerShop.Application.Features.Deliverers.Queries.GetDeliverersSummary;

public class GetDeliverersSummaryHandler(IDelivererRepository delivererRepo) : IHandler
{
    public async Task<GetDeliverersSummaryResponse> Handle(GetDeliverersSummaryQuery request, CancellationToken ct = default)
    {
        var pagedDeliverers = await delivererRepo.GetPagedDeliverersAsync(
            request.Search,
            request.SortBy,
            request.VehicleType,
            request.DelivererStatus,
            request.Page,
            request.PageSize,
            ct);

        var statistics = await delivererRepo.GetStatisticsAsync(ct);

        return new GetDeliverersSummaryResponse
        {
            PagedDeliverers = pagedDeliverers,
            Statistics = statistics
        };
    }
}
