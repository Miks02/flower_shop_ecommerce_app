using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.SharedKernel.Results;
using Microsoft.AspNetCore.Identity;

namespace FlowerShop.Application.Features.Users.Commands.RemoveProfilePicture;

public class RemoveProfilePictureHandler(UserManager<User> userManager) : IHandler
{
    public async Task<Result> Handle(RemoveProfilePictureCommand command)
    {
        var user = await userManager.FindByIdAsync(command.UserId);

        if (user is null)
            return Result.Failure(UserError.NotFound(command.UserId));

        if (string.IsNullOrWhiteSpace(user.ImagePath))
            return Result.Success();

        user.ImagePath = null;

        var updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var errors = updateResult.Errors.Select(e => new Error(e.Code, e.Description));
            return Result.Failure(errors.ToArray());
        }
        
        return Result.Success();
    }
}