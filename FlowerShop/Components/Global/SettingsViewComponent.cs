using System.Runtime.InteropServices.JavaScript;
using FlowerShop.Models;
using FlowerShop.ViewModels.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Components.Global;

[Authorize]
public class SettingsViewComponent : ViewComponent
{

    private readonly UserManager<ApplicationUser> _userManager;

    public SettingsViewComponent(UserManager<ApplicationUser> userManager)
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

        var profileVm = new ProfileSettingsViewModel()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName!,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            FullNameInitials = user.FirstName[0].ToString() + user.LastName[0],
            ImagePath = user.ImagePath
        };

        var vm = new SettingsPageViewModel()
        {
            ProfileVm = profileVm,
            ChangePasswordVm = new ChangePasswordViewModel()
        };

        ViewBag.Disabled = string.IsNullOrEmpty(user.ImagePath);
        
        return await Task.FromResult(View(vm));
    }
}