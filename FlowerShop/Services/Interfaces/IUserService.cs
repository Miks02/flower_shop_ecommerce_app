using FlowerShop.Models;
using FlowerShop.Services.Results;
using FlowerShop.ViewModels.Components;

namespace FlowerShop.Services.Interfaces;

public interface IUserService
{
    Task<ApplicationUser?> GetCurrentUser();

    Task<ProfileUpdateResult> UpdateProfileAsync(SettingsPageViewModel model);

}