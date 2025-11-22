using FlowerShop.Models;
using FlowerShop.Services.Results;
using FlowerShop.ViewModels.Components;

namespace FlowerShop.Services.Interfaces;

public interface IUserService
{
    Task<ApplicationUser?> GetCurrentUser();
    
    Task<ApplicationUser?> GetUserByIdAsync(string userId);

    Task<ApplicationUser?> GetUserByNameAsync(string userName);

    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    
    Task<OperationResult<ApplicationUser>> CreateUserAsync(RegisterViewModel model, string role);

    Task<ProfileUpdateResult> UpdateProfileAsync(SettingsPageViewModel model);

    Task<OperationResult<ApplicationUser>> DeleteUserAsync(ApplicationUser user);
    
    Task<OperationResult<ApplicationUser>> DeleteUserAsync(string userId);

    Task<OperationResult<ApplicationUser>> RemoveProfilePictureAsync(string userId);

}