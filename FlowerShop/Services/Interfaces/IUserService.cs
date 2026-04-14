using FlowerShop.Web.Dto.User;
using FlowerShop.Web.Models;
using FlowerShop.Web.Services.Results;
using FlowerShop.Web.ViewModels.Components;

namespace FlowerShop.Web.Services.Interfaces;

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