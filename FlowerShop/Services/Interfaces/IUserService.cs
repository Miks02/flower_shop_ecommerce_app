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
    
    Task<Result<ApplicationUser>> CreateUserAsync(RegisterViewModel model, string role);

    Task<Result<ProfileUpdateDto>> UpdateProfileAsync(SettingsPageViewModel model);

    Task<Result> DeleteUserAsync(ApplicationUser user);
    
    Task<Result> DeleteUserAsync(string userId);

    Task<Result> RemoveProfilePictureAsync(string userId);

}