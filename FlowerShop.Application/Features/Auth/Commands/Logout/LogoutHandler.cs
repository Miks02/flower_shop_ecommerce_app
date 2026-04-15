using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.IdentityUser;
using Microsoft.AspNetCore.Identity;

namespace FlowerShop.Application.Features.Auth.Commands.Logout;

public class LogoutHandler(SignInManager<User> signInManager) : IHandler
{
    public async Task Handle()
    {
        await signInManager.SignOutAsync();
    }
}