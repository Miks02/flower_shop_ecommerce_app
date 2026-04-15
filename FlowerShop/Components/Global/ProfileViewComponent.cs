using System.Security.Claims;
using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Common.Abstractions.Dto;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.Web.Models;
using FlowerShop.Web.Services.Interfaces;
using FlowerShop.Web.ViewModels.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Components.Global;

[Authorize]
public class ProfileViewComponent(IUserProvider userProvider) : ViewComponent
{

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = userProvider.GetCurrentUserId();
        var user = await userProvider.GetCurrentUserDetails(userId);
        
        if (User.IsInRole("User"))
        {
            var viewModel = CreateUserProfileViewModel(user);
            return View("User/Default", viewModel);
        }
        
        
        return View("NotFound");
    }

    private UserProfileViewModel CreateUserProfileViewModel(UserDetailsDto user)
    {
        return new UserProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Username = user.Username,
            ProfilePicture = Url.Content(user.ProfilePicture),
            Address = string.Empty,
            Status = user.Status,
            RegistrationDate = user.RegistrationDate.ToString("dd.MM.yyyy"),
        };
    }
}