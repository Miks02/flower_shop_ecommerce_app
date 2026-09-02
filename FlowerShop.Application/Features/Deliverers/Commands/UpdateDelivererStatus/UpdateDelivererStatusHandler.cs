using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Deliverers.Commands.UpdateDelivererStatus;

public class UpdateDelivererStatusHandler(
    IDelivererRepository delivererRepo,
    IUnitOfWork unitOfWork) : IHandler
{
    public async Task<Result> Handle(UpdateDelivererStatusCommand command, CancellationToken ct = default)
    {
        var deliverer = await delivererRepo.GetByIdAsync(command.Id, ct);
        if (deliverer is null)
            return Result.Failure(DelivererError.NotFound(command.Id));

        if (deliverer.DelivererStatus == command.Status)
            return Result.Success();

        deliverer.DelivererStatus = command.Status;

        delivererRepo.Update(deliverer);
        await unitOfWork.SaveAsync(ct);

        return Result.Success();
    }
}
