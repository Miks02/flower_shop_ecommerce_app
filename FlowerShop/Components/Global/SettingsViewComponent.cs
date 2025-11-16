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

        var vm = new SettingsViewModel()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.UserName!,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            ProfilePicture = user.ImagePath,
        };
        
        return await Task.FromResult(View(vm));
    }
}