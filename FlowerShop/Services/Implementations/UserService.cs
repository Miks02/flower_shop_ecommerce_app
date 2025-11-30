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

    public async Task<OperationResult<ApplicationUser>> CreateUserAsync(RegisterViewModel model, string role)
    {
        var result = new OperationResult<ApplicationUser>();

        var credentialsCheckResult = await IsUserTaken(model.Username, model.Email);

        if (!credentialsCheckResult.Succeeded)
        {
            result.Errors.AddRange(credentialsCheckResult.Errors);
            LogError("Creating user has failed. Credentials check failed");
            return result;
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
            result.Errors.Add("Došlo je do greške prilikom registracije korisnika.");
            LogError("Creating user has failed. Unexpected error happened");

            foreach (var error in userCreateResult.Errors)
                LogError("ERROR: " + error.Description);
            return result;
        }
        
        var roleAssignResult = await AssignRole(user, role);

        if (!roleAssignResult.Succeeded)
        {
            result.Errors.AddRange(roleAssignResult.Errors);
            LogError("Creating user succeeded. But assigning role failed.");
            LogInfo("Attempting to delete newly created user from the database...");
            
            await DeleteUserAsync(user);
            
            return result;
        }
        
        LogInfo($"User \"{user.UserName}\" has been created successfully!");
        result.Payload = user;
        return result;
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

    public async Task<OperationResult<ApplicationUser>> RemoveProfilePictureAsync(string userId)
    {
        var user = await GetUserByIdAsync(userId);
        var result = new OperationResult<ApplicationUser>();
        
        if (user is null)
            throw new KeyNotFoundException("User not found.");

        if (user.ImagePath is null)
        {
            result.Errors.Add("Korisnik nema profilnu sliku za brisanje");
            return result;
        }

        string oldImagePath = user.ImagePath;

        user.ImagePath = null;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            result.Errors.Add("Došlo je do greške prilikom brisanja profilne slike.");
            LogError("Unexpected error happened while deleting profile picture");
            foreach (var error in  updateResult.Errors)
            {
                LogError("ERROR: " + error.Description);
            }

            return result;
        }

        if(!_fileService.DeleteFile(oldImagePath))
            LogError("User's old profile picture could not be removed from system's files.");
        
        LogInfo("User's profile picture has been removed from the application successfully.");
        return result;

    }

    public async Task<OperationResult<ApplicationUser>> DeleteUserAsync(ApplicationUser user)
    {
        var result = new OperationResult<ApplicationUser>();
        
        var deleteResult = await _userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
        {
            result.Errors.Add("Došlo je do greške prilikom brisanja korisnika.");
            LogError("Unexpected error happened while deleting user from database");
            foreach (var error in deleteResult.Errors)
                LogError("ERROR: " + error.Description);
        }
        
        LogInfo("User has been deleted successfully.");
        return result;
    }

    public async Task<OperationResult<ApplicationUser>> DeleteUserAsync(string userId)
    {
        var result = new OperationResult<ApplicationUser>();
        var user = await GetUserByIdAsync(userId);

        if (user is null)
        {
            result.Errors.Add("Korisnik nije pronadjen");
            LogError("User not found");
            return result;       
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

    private async Task<OperationResult<ApplicationUser>> AssignRole(ApplicationUser user, string role)
    {
        var result = new OperationResult<ApplicationUser>();
        
        if (!IsValidRole(role))
        {
            result.Errors.Add("Nevalidna korisnička rola");
            LogError("Invalid user role entered");
            return result;
        }
        
        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole(role));
            
        var roleCreateResult = await _userManager.AddToRoleAsync(user, role);

        if (!roleCreateResult.Succeeded)
        {
            result.Errors.Add("Došlo je do greške prilikom dodavanja korisnika u rolu.");
            LogError("Unexpected error happened while assigning user role");
            foreach (var error in roleCreateResult.Errors)
                LogError("ERROR: " + error.Description);
            
            return result;
        }
        LogInfo("User assigned to role successfully.");
        return result;
    }

    private async Task<OperationResult<ApplicationUser>> IsUserTaken(string username, string email)
    {
        var result = new OperationResult<ApplicationUser>();
        
        if (await GetUserByEmailAsync(email) is not null)
        {
            LogError("Email address is taken");
            result.Errors.Add("Email adresa je zauzeta");
            return result;
        }
        
        if (await GetUserByNameAsync(username) is not null)
        {
            result.Errors.Add("Korisničko ime je zauzeto");
            LogError("Username is taken");
            return result;
        }

        LogInfo("Credentials check passed successfully.");
        return result;
    }
}