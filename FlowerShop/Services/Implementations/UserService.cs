using System.Net;
using System.Security.Claims;
using FlowerShop.Models;
using FlowerShop.Services.Interfaces;
using FlowerShop.Services.Results;
using FlowerShop.ViewModels.Components;
using Microsoft.AspNetCore.Identity;

namespace FlowerShop.Services.Implementations;

public class UserService : IUserService
{
    
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFileService _fileService;
    private readonly ILogger<UserService> _logger;
    
    public UserService(IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager,
        IFileService fileService,
        ILogger<UserService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _fileService = fileService;
        _logger = logger;
    }

    private string? CurrentUserId => _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    public async Task<ApplicationUser?> GetCurrentUser()
    {
        if (CurrentUserId is null)
            return null;
        
        return await _userManager.FindByIdAsync(CurrentUserId);
    }

    public async Task<ProfileUpdateResult> UpdateProfileAsync(SettingsPageViewModel model)
    {
        var result = new ProfileUpdateResult();
        var user = await GetCurrentUser();

        if (user is null)
            throw new KeyNotFoundException("Korisnik nije pronadjen");
        
        bool requiresUserUpdate =
            user.UserName != model.ProfileVm.UserName ||
            user.Email != model.ProfileVm.Email ||
            user.FirstName != model.ProfileVm.FirstName ||
            user.LastName != model.ProfileVm.LastName ||
            user.PhoneNumber != model.ProfileVm.PhoneNumber ||
            model.ProfileVm.ProfilePicture != null;

        if (requiresUserUpdate)
        {
            if (user.Email != model.ProfileVm.Email)
            {
                var existingEmailUser = await _userManager.FindByEmailAsync(model.ProfileVm.Email);
                if (existingEmailUser is not null && existingEmailUser.Id != user.Id)
                {
                    result.Errors.Add("Email adresa je zauzeta");
                    return result;
                }
            }

            if (user.UserName != model.ProfileVm.UserName)
            {
                var existingUserNameUser = await _userManager.FindByNameAsync(model.ProfileVm.UserName);
                if (existingUserNameUser is not null && existingUserNameUser.Id != user.Id)
                {
                    result.Errors.Add("Korisničko ime je zauzeto");
                    return result;
                }
            }
            
            user.FirstName = model.ProfileVm.FirstName;
            user.LastName = model.ProfileVm.LastName;
            user.Email = model.ProfileVm.Email;
            user.UserName = model.ProfileVm.UserName;
            user.PhoneNumber = model.ProfileVm.PhoneNumber;

            if (model.ProfileVm.ProfilePicture != null && model.ProfileVm.ProfilePicture.Length > 0)
                user.ImagePath = await _fileService.UploadFile(model.ProfileVm.ProfilePicture, user.ImagePath, "Users");
            
            
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                result.Errors.Add("Došlo je do greške prilikom ažuriranja podataka");
                foreach (var error in updateResult.Errors)
                {
                    _logger.LogError("ERROR: " + error);
                }
                return result;
            }
            
            _logger.LogInformation("Profile updated successfully for user: " + user.UserName);
            result.ProfileUpdated = true;
        }
        
        if (!string.IsNullOrWhiteSpace(model.ChangePasswordVm.CurrentPassword))
        {
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.ChangePasswordVm.CurrentPassword);

            if (!passwordValid)
            {
                result.Errors.Add("Uneta trenutna lozinka nije ispravna.");
                return result;
            }

            var passwordResult = await _userManager.ChangePasswordAsync(
                user,
                model.ChangePasswordVm.CurrentPassword,
                model.ChangePasswordVm.NewPassword
            );

            if (!passwordResult.Succeeded)
            {
                result.Errors.Add("Došlo je do greške prilikom ažuriranja lozinke");
                
                _logger.LogError("Unexpected error happened while changing password for user: " + user.UserName);
                foreach (var error in passwordResult.Errors)
                {
                    _logger.LogError("ERROR: " + error);
                }
                return result;
            }
            _logger.LogInformation("Password updated successfully for user: " + user.UserName);
            result.PasswordChanged = true;
        }

        return result;
            

    }
    
    
}