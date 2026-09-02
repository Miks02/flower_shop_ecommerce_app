using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Deliverers.Queries.GetDeliverers;

public class GetDeliverersHandler(IDelivererRepository delivererRepo) : IHandler
{
    public async Task<PagedResult<DelivererDto>> Handle(GetDeliverersQuery request, CancellationToken ct = default)
    {
        return await delivererRepo.GetPagedDeliverersAsync(
            request.Search,
            request.SortBy,
            request.VehicleType,
            request.DelivererStatus,
            request.Page,
            request.PageSize,
            ct);
    }
}
