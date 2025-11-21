using FlowerShop.Models;
using FlowerShop.Services.Results;
using FlowerShop.ViewModels.Components;

namespace FlowerShop.Services.Interfaces;

public interface IUserService
{
    Task<ApplicationUser?> GetCurrentUser();
    
    Task<ApplicationUser?> GetUserByIdAsync(string userId);

    Task<ApplicationUser?> GetUserByNameAsync(string userName);

    Task<ProfileUpdateResult> UpdateProfileAsync(SettingsPageViewModel model);

    Task<OperationResult> RemoveProfilePictureAsync(string userId);

}