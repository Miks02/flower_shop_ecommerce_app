using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.SharedKernel.Results;
using Microsoft.AspNetCore.Identity;

namespace FlowerShop.Application.Features.Deliverers.Commands.DeleteDeliverer;

public class DeleteDelivererHandler(
    IDelivererRepository delivererRepo,
    UserManager<User> userManager) : IHandler
{
    public async Task<Result> Handle(DeleteDelivererCommand command, CancellationToken ct = default)
    {
        var delivererToDelete = await delivererRepo.GetByIdAsync(command.Id, ct);
        
        if (delivererToDelete is null || !await userManager.IsInRoleAsync(delivererToDelete.User, "Deliverer"))
            return Result.Failure(DelivererError.NotFound(command.Id));
        
        if(delivererToDelete.IsOnDuty())
            return Result.Failure(DelivererError.CannotDeleteWhileOnDuty(command.Id));

        var deleteResult = await userManager.DeleteAsync(delivererToDelete.User);

        if (!deleteResult.Succeeded)
        {
            var errors = deleteResult.Errors
                .Select(x => new Error(x.Code, x.Description))
                .ToArray();
            
            return Result.Failure(errors);
        }
        
        return Result.Success();
    }
}
