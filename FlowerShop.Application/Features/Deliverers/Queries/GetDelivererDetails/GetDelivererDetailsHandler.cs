using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Deliverers.Queries.GetDelivererDetails;

public class GetDelivererDetailsHandler(IDelivererRepository delivererRepo, IOrderRepository orderRepo) : IHandler
{
    public async Task<Result<GetDelivererDetailsResponse>> Handle(GetDelivererDetailsQuery detailsQuery, CancellationToken ct = default)
    {
        var deliverer = await delivererRepo.GetByIdAsync(detailsQuery.Id, ct);
        
        if(deliverer is null)
            return Result<GetDelivererDetailsResponse>.Failure(DelivererError.NotFound(detailsQuery.Id));
        
        var (totalDeliveries, activeDeliveries, completedDeliveries, averageRating) = await orderRepo.GetDelivererOrderStatsAsync(deliverer.Id, ct);

        var response = new GetDelivererDetailsResponse
        {
            DelivererId = deliverer.Id,
            FirstName = deliverer.User.FirstName,
            LastName = deliverer.User.LastName,
            Email = deliverer.User.Email ?? string.Empty,
            PhoneNumber = deliverer.User.PhoneNumber ?? string.Empty,
            ProfilePicture = deliverer.User.ImagePath,
            AccountStatus = deliverer.User.AccountStatus,
            DelivererStatus = deliverer.DelivererStatus,
            VehicleType = deliverer.VehicleType,
            RegistrationDate = deliverer.User.CreatedAt.ToString("dd.MM.yyyy"),
            AverageRating = averageRating,
            TotalDeliveries = totalDeliveries,
            ActiveDeliveries = activeDeliveries,
            CompletedDeliveries = completedDeliveries
        };
        
        return Result<GetDelivererDetailsResponse>.Success(response);
    }
}