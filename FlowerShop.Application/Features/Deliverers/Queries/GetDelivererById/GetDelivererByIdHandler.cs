using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Deliverers.Queries.GetDelivererById;

public class GetDelivererByIdHandler(IDelivererRepository delivererRepo) : IHandler
{
    public async Task<Result<GetDelivererByIdResponse>> Handle(string id, CancellationToken ct = default)
    {
        var deliverer = await delivererRepo.GetByIdAsync(id, ct);
        if (deliverer is null)
            return Result<GetDelivererByIdResponse>.Failure(DelivererError.NotFound(id));

        var response = new GetDelivererByIdResponse
        {
            Id = deliverer.Id,
            FirstName = deliverer.User?.FirstName ?? string.Empty,
            LastName = deliverer.User?.LastName ?? string.Empty,
            Email = deliverer.User?.Email ?? string.Empty,
            PhoneNumber = deliverer.User?.PhoneNumber ?? string.Empty,
            VehicleType = deliverer.VehicleType,
            DelivererStatus = deliverer.DelivererStatus,
            CreatedAt = deliverer.User?.CreatedAt ?? DateTime.UtcNow
        };

        return Result<GetDelivererByIdResponse>.Success(response);
    }
}
