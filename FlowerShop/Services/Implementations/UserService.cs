using System.Runtime.InteropServices.JavaScript;
using FlowerShop.Dto.User;
using FlowerShop.Models;
using FlowerShop.Services.Interfaces;
using FlowerShop.Services.Results;
using FlowerShop.ViewModels.Components;
using Microsoft.AspNetCore.Identity;

namespace FlowerShop.Services.Implementations;

public class UserService : BaseService<UserService>, IUserService
{
    
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IFileService _fileService;
    
    public UserService(IHttpContextAccessor http,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IFileService fileService,
        ILogger<UserService> logger) : base(http, logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _fileService = fileService;
    }
    
    public async Task<ApplicationUser?> GetCurrentUser()
    {
        if (CurrentUserId is null)
            return null;
        
        return await _userManager.FindByIdAsync(CurrentUserId);
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<ApplicationUser?> GetUserByNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<ServiceResult<ApplicationUser>> CreateUserAsync(RegisterViewModel model, string role)
    {
        var credentialsCheckResult = await IsUserTaken(model.Username, model.Email);

        if (!credentialsCheckResult.IsSucceeded)
        {
            LogError("Creating user has failed. Credentials check failed");
            return ServiceResult<ApplicationUser>.Failure(Error.Auth.InvalidCredentials(credentialsCheckResult.Errors![0].Description));
        }

        var user = new ApplicationUser()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = model.Username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
        };

        var userCreateResult = await _userManager.CreateAsync(user, model.Password);

        if (!userCreateResult.Succeeded)
        {
            LogError("Creating user has failed. Unexpected error happened");

            foreach (var error in userCreateResult.Errors)
                LogError("ERROR: " + error.Description);
            return ServiceResult<ApplicationUser>.Failure(Error.Database.QueryError("Došlo je do greške prilikom registracije korisnika."));
        }
        
        var roleAssignResult = await AssignRole(user, role);

        if (!roleAssignResult.IsSucceeded)
        {
            var errors = roleAssignResult.Errors!;
            LogError("Creating user succeeded. But assigning role failed.");
            LogInfo("Attempting to delete newly created user from the database...");
            
            await DeleteUserAsync(user);
            
            return ServiceResult<ApplicationUser>.Failure(errors.ToArray());
        }
        
        LogInfo($"User \"{user.UserName}\" has been created successfully!");
        return ServiceResult<ApplicationUser>.Success(user);
    }

    public async Task<ServiceResult<ProfileUpdateDto>> UpdateProfileAsync(SettingsPageViewModel model)
    {
        var profileUpdateResult = new ProfileUpdateDto();
        var user = await GetCurrentUser();

        if (user is null)
            return ServiceResult<ProfileUpdateDto>.Failure(Error.User.NotFound());

        profileUpdateResult.User = user;
        
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
                    return ServiceResult<ProfileUpdateDto>.Failure(Error.User.EmailAlreadyExists());
                }
            }

            if (user.UserName != model.ProfileVm.UserName)
            {
                var existingUserNameUser = await _userManager.FindByNameAsync(model.ProfileVm.UserName);
                if (existingUserNameUser is not null && existingUserNameUser.Id != user.Id)
                {
                    return ServiceResult<ProfileUpdateDto>.Failure(Error.User.UsernameAlreadyExists(model.ProfileVm.UserName));
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
                LogError("Error happened while updating user data");
                foreach (var error in updateResult.Errors)
                {
                    LogError("ERROR: " + error.Description);
                }
                return ServiceResult<ProfileUpdateDto>.Failure(Error.Database.QueryError("Došlo je do greške prilikom ažuriranja podataka."));
            }
            
            LogInfo("Profile updated successfully");
            profileUpdateResult.ProfileUpdated = true;
        }
        
        if (!string.IsNullOrWhiteSpace(model.ChangePasswordVm.CurrentPassword))
        {
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.ChangePasswordVm.CurrentPassword);

            if (!passwordValid)
            {
                LogError("Password changing failed. Incorrect current password entered");
                return ServiceResult<ProfileUpdateDto>.Failure(Error.Validation.InvalidInput("Uneta trenutna lozinka nije validna"));
            }

            var passwordResult = await _userManager.ChangePasswordAsync(
                user,
                model.ChangePasswordVm.CurrentPassword,
                model.ChangePasswordVm.NewPassword
            );

            if (!passwordResult.Succeeded)
            {
                LogError("Changing password failed. Unexpected error happened");
                foreach (var error in passwordResult.Errors)
                {
                    LogError("ERROR: " + error.Description);
                }
                return ServiceResult<ProfileUpdateDto>.Failure(Error.Database.QueryError("Došlo je do greške prilikom promene lozinke."));
            }
            LogInfo("Password changed successfully");
            profileUpdateResult.PasswordChanged = true;
        }

        return ServiceResult<ProfileUpdateDto>.Success(profileUpdateResult);
    }

    public async Task<ServiceResult> RemoveProfilePictureAsync(string userId)
    {
        var user = await GetUserByIdAsync(userId);
        
        if (user is null)
            return ServiceResult.Failure(Error.User.NotFound());

        if (user.ImagePath is null)
            return ServiceResult.Success();
        

        string oldImagePath = user.ImagePath;

        user.ImagePath = null;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            LogError("Unexpected error happened while deleting profile picture");
            foreach (var error in  updateResult.Errors)
            {
                LogError("ERROR: " + error.Description);
            }

            return ServiceResult.Failure(Error.Database.QueryError("Došlo je do greške prilikom brisanja profilne slike."));
        }

        if(!_fileService.DeleteFile(oldImagePath))
            LogError("User's old profile picture could not be removed from system's files.");
        
        LogInfo("User's profile picture has been removed from the application successfully.");
        return ServiceResult.Success();

    }

    public async Task<ServiceResult> DeleteUserAsync(ApplicationUser user)
    {
        
        var deleteResult = await _userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
        {
            LogError("Unexpected error happened while deleting user from database");
            foreach (var error in deleteResult.Errors)
                LogError("ERROR: " + error.Description);
            return ServiceResult.Failure(Error.Database.QueryError("Došlo je do greške prilikom brisanja korisnika."));
        }
        
        LogInfo("User has been deleted successfully.");
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteUserAsync(string userId)
    {
        var user = await GetUserByIdAsync(userId);

        if (user is null)
        {
            LogError("User not found");
            return ServiceResult.Failure(Error.User.NotFound());       
        }
        
        return await DeleteUserAsync(user);
    }
    
    private bool IsValidRole(string role)
    {
        string[] allowedRoles = ["User", "DeliveryPerson"];

        if (!allowedRoles.Contains(role))
            return false;

        return true;
    }

    private async Task<ServiceResult> AssignRole(ApplicationUser user, string role)
    {
        
        if (!IsValidRole(role))
        {
            LogError("Invalid user role entered");
            return ServiceResult.Failure(Error.Validation.InvalidInput("Nevažeća korisnička rola"));
        }
        
        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole(role));
            
        var roleCreateResult = await _userManager.AddToRoleAsync(user, role);

        if (!roleCreateResult.Succeeded)
        {
            LogError("Unexpected error happened while assigning user role");
            foreach (var error in roleCreateResult.Errors)
                LogError("ERROR: " + error.Description);
            
            return ServiceResult.Failure(Error.Database.QueryError("Došlo je do greške prilikom dodavanja korisnika u rolu."));
        }
        LogInfo("User assigned to role successfully.");
        return ServiceResult.Success();
    }

    private async Task<ServiceResult> IsUserTaken(string username, string email)
    {
        
        if (await GetUserByEmailAsync(email) is not null)
        {
            LogError("Email address is taken");
            return ServiceResult.Failure(Error.User.EmailAlreadyExists());
        }
        
        if (await GetUserByNameAsync(username) is not null)
        {
            LogError("Username is taken");
            return ServiceResult.Failure(Error.User.UsernameAlreadyExists(username)); 
        }

        LogInfo("Credentials check passed successfully.");
        return ServiceResult.Success();
    }
}