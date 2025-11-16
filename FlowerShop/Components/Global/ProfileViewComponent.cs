using FlowerShop.Models;
using FlowerShop.ViewModels.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Components.Global;

[Authorize]
public class ProfileViewComponent : ViewComponent
{

    private readonly UserManager<ApplicationUser> _userManager;
    
    public ProfileViewComponent(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userName = User.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
            return View("Error");
        
        
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null)
            return View("Error");
        
        
        if (User.IsInRole("User"))
        {
            var viewModel = CreateUserProfileViewModel(user);
            return View("User/Default", viewModel);
        }
        
        
        return View("NotFound");
    }

    private UserProfileViewModel CreateUserProfileViewModel(ApplicationUser user)
    {
        return new UserProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            Username = user.UserName!,
            ProfilePicture = Url.Content(user.ImagePath),
            Address = string.Empty,
            Status = user.AccountStatus,
            RegistrationDate = user.CreatedAt.ToString("dd.MM.yyyy"),
        };
    }
}