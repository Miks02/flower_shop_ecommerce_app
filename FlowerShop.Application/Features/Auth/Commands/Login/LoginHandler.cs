using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.SharedKernel.ErrorCatalogue;
using FlowerShop.SharedKernel.Results;
using Microsoft.AspNetCore.Identity;

namespace FlowerShop.Application.Features.Auth.Commands.Login;

public class LoginHandler(SignInManager<User> signInManager) : IHandler
{
    public async Task<Result> Handle(LoginCommand command)
    {
        var result = await signInManager.PasswordSignInAsync(command.Username, command.Password, command.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
            return Result.Success();

        
        return Result.Failure(AuthError.LoginFailed());
    }
}