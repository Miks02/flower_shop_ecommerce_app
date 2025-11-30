using FlowerShop.Dto.User;
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
    
    Task<ServiceResult<ApplicationUser>> CreateUserAsync(RegisterViewModel model, string role);

    Task<ServiceResult<ProfileUpdateDto>> UpdateProfileAsync(SettingsPageViewModel model);

    Task<ServiceResult> DeleteUserAsync(ApplicationUser user);
    
    Task<ServiceResult> DeleteUserAsync(string userId);

    Task<ServiceResult> RemoveProfilePictureAsync(string userId);

}