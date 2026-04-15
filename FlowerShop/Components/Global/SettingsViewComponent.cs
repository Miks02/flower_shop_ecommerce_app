using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Web.ViewModels.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Components.Global;

[Authorize]
public class SettingsViewComponent(IUserProvider userProvider) : ViewComponent
{
    
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await userProvider.GetCurrentUserDetails(userProvider.GetCurrentUserId());
        
        var profileVm = new ProfileSettingsViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FullNameInitials = user.FirstName[0].ToString() + user.LastName[0],
            ImagePath = user.ProfilePicture
        };
        
        var vm = new SettingsPageViewModel()
        {
            ProfileVm = profileVm,
            ChangePasswordVm = new ChangePasswordViewModel()
        };
        
        return await Task.FromResult(View(vm));
    }
}