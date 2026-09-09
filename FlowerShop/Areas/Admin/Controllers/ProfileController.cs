using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Web.ViewModels.Components;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProfileController(IUserProvider userProvider) : Controller
{
    private const string SettingsPartial = "~/Views/Shared/_SettingsPartial.cshtml";

    [HttpGet]
    public async Task<IActionResult> Settings(CancellationToken ct = default)
    {
        var vm = await BuildSettingsViewModelAsync(ct);

        if (Request.IsHtmx())
            return PartialView(SettingsPartial, vm);

        return View(vm);
    }

    private async Task<SettingsPageViewModel> BuildSettingsViewModelAsync(CancellationToken ct)
    {
        var user = await userProvider.GetCurrentUserDetails(userProvider.GetCurrentUserId(), ct);

        return new SettingsPageViewModel
        {
            ProfileVm = new ProfileSettingsViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullNameInitials = user.FirstName[0].ToString() + user.LastName[0],
                ImagePath = user.ProfilePicture
            },
            ChangePasswordVm = new ChangePasswordViewModel()
        };
    }
}
