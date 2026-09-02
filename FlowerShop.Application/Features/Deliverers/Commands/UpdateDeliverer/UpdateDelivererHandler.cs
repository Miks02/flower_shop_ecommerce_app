using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.SharedKernel.Results;
using Microsoft.AspNetCore.Identity;

namespace FlowerShop.Application.Features.Deliverers.Commands.UpdateDeliverer;

public class UpdateDelivererHandler(
    IDelivererRepository delivererRepo,
    UserManager<User> userManager,
    IUnitOfWork unitOfWork) : IHandler
{
    public async Task<Result> Handle(UpdateDelivererCommand command, CancellationToken ct = default)
    {
        var deliverer = await delivererRepo.GetByIdAsync(command.Id, ct);
        if (deliverer is null)
            return Result.Failure(DelivererError.NotFound(command.Id));

        var user = deliverer.User;

        if (!string.Equals(user.Email, command.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existingUser = await userManager.FindByEmailAsync(command.Email);
            if (existingUser is not null && existingUser.Id != command.Id)
                return Result.Failure(DelivererError.EmailAlreadyExists(command.Email));

            user.Email = command.Email;
            user.UserName = command.Email;
            user.NormalizedEmail = command.Email.ToUpperInvariant();
            user.NormalizedUserName = command.Email.ToUpperInvariant();
        }

        user.FirstName = command.FirstName;
        user.LastName = command.LastName;
        user.PhoneNumber = command.PhoneNumber;

        deliverer.VehicleType = command.VehicleType;
        deliverer.DelivererStatus = command.DelivererStatus;

        delivererRepo.Update(deliverer);
        await unitOfWork.SaveAsync(ct);

        return Result.Success();
    }
}
