using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.SharedKernel.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FlowerShop.Application.Features.Users.Commands.UpdatePassword;

public class UpdatePasswordHandler(UserManager<User> userManager, ILogger<UpdatePasswordHandler> logger) : IHandler
{
    public async Task<Result> Handle(UpdatePasswordCommand command)
    {
        var user = await userManager.FindByIdAsync(command.UserId);

        if (user is null)
        {
            return Result.Failure(UserError.NotFound(command.UserId));
        }

        var result = await userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);

        if (!result.Succeeded)
        {
            logger.LogWarning("An error occurred while trying to update user's password. UserID: {UserId}", command.UserId);
            
            var errors = result.Errors
                .Select(e => new Error(e.Code, e.Description))
                .ToArray();

            return Result.Failure(errors);
        }

        return Result.Success();
    }
}