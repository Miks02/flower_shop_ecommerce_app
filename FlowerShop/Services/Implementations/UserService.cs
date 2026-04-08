using System.Security.Claims;
using FlowerShop.Dto.User;
using FlowerShop.Models;
using FlowerShop.Services.Interfaces;
using FlowerShop.Services.Results;
using FlowerShop.ViewModels.Components;
using Microsoft.AspNetCore.Identity;

namespace FlowerShop.Services.Implementations;

public class UserService(
    IHttpContextAccessor http,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IFileService fileService,
    ILogger<UserService> logger) : IUserService
{
    private string? CurrentUserId => http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

    public async Task<ApplicationUser?> GetCurrentUser()
    {
        if (CurrentUserId is null)
            return null;
        
        return await userManager.FindByIdAsync(CurrentUserId);
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await userManager.FindByIdAsync(userId);
    }

    public async Task<ApplicationUser?> GetUserByNameAsync(string userName)
    {
        return await userManager.FindByNameAsync(userName);
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<Result<ApplicationUser>> CreateUserAsync(RegisterViewModel model, string role)
    {
        var credentialsCheckResult = await IsUserTaken(model.Username, model.Email);

        if (!credentialsCheckResult.IsSucceeded)
        {
            logger.LogError("Creating user has failed. Credentials check failed");
            return Result<ApplicationUser>.Failure(Error.Auth.InvalidCredentials(credentialsCheckResult.Errors![0].Description));
        }

        var user = new ApplicationUser()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = model.Username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
        };

        var userCreateResult = await userManager.CreateAsync(user, model.Password);

        if (!userCreateResult.Succeeded)
        {
            logger.LogError("Creating user has failed. Unexpected error happened");

            foreach (var error in userCreateResult.Errors) 
                logger.LogError("ERROR: {err}", error.Description);
            return Result<ApplicationUser>.Failure(Error.Database.QueryError("Došlo je do greške prilikom registracije korisnika."));
        }
        
        var roleAssignResult = await AssignRole(user, role);

        if (!roleAssignResult.IsSucceeded)
        {
            var errors = roleAssignResult.Errors!;
            logger.LogError("Creating user succeeded. But assigning role failed.");
            logger.LogInformation("Attempting to delete newly created user from the database...");
            
            await DeleteUserAsync(user);
            
            return Result<ApplicationUser>.Failure(errors.ToArray());
        }
        
        logger.LogInformation("User \"{username}\" has been created successfully!", user.UserName);
        return Result<ApplicationUser>.Success(user);
    }

    public async Task<Result<ProfileUpdateDto>> UpdateProfileAsync(SettingsPageViewModel model)
    {
        var profileUpdateResult = new ProfileUpdateDto();
        var user = await GetCurrentUser();

        if (user is null)
            return Result<ProfileUpdateDto>.Failure(Error.User.NotFound());

        profileUpdateResult.User = user;
        
        if(!RequiresUserUpdate(model.ProfileVm, user))
            return Result<ProfileUpdateDto>.Success(profileUpdateResult);
        
        if (user.Email != model.ProfileVm.Email)
        {
            var existingEmailUser = await userManager.FindByEmailAsync(model.ProfileVm.Email);
            if (existingEmailUser is not null && existingEmailUser.Id != user.Id)
            {
                return Result<ProfileUpdateDto>.Failure(Error.User.EmailAlreadyExists());
            }
        }

        if (user.UserName != model.ProfileVm.UserName)
        {
            var existingUserNameUser = await userManager.FindByNameAsync(model.ProfileVm.UserName);
            if (existingUserNameUser is not null && existingUserNameUser.Id != user.Id)
            {
                return Result<ProfileUpdateDto>.Failure(Error.User.UsernameAlreadyExists(model.ProfileVm.UserName));
            }
        }
            
        user.FirstName = model.ProfileVm.FirstName;
        user.LastName = model.ProfileVm.LastName;
        user.Email = model.ProfileVm.Email;
        user.UserName = model.ProfileVm.UserName;
        user.PhoneNumber = model.ProfileVm.PhoneNumber;

        if (model.ProfileVm.ProfilePicture != null && model.ProfileVm.ProfilePicture.Length > 0)
            user.ImagePath = await fileService.UploadFile(model.ProfileVm.ProfilePicture, user.ImagePath, "Users");
            
            
        var updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            logger.LogError("Error happened while updating user data");
            foreach (var error in updateResult.Errors)
            {
                logger.LogError("ERROR: {err}", error.Description);
            }
            return Result<ProfileUpdateDto>.Failure(Error.Database.QueryError("Došlo je do greške prilikom ažuriranja podataka."));
        }
            
        logger.LogInformation("Profile updated successfully");
        profileUpdateResult.ProfileUpdated = true;
        
        
        if (!string.IsNullOrWhiteSpace(model.ChangePasswordVm.CurrentPassword))
        {
            var passwordValid = await userManager.CheckPasswordAsync(user, model.ChangePasswordVm.CurrentPassword);

            if (!passwordValid)
            {
                logger.LogError("Password changing failed. Incorrect current password entered");
                return Result<ProfileUpdateDto>.Failure(Error.Validation.InvalidInput("Uneta trenutna lozinka nije validna"));
            }

            var passwordResult = await userManager.ChangePasswordAsync(
                user,
                model.ChangePasswordVm.CurrentPassword,
                model.ChangePasswordVm.NewPassword
            );

            if (!passwordResult.Succeeded)
            {
                logger.LogError("Changing password failed. Unexpected error happened");
                foreach (var error in passwordResult.Errors)
                {
                    logger.LogError("ERROR: {err}", error.Description);
                }
                return Result<ProfileUpdateDto>.Failure(Error.Database.QueryError("Došlo je do greške prilikom promene lozinke."));
            }
            logger.LogInformation("Password changed successfully");
            profileUpdateResult.PasswordChanged = true;
        }

        return Result<ProfileUpdateDto>.Success(profileUpdateResult);
    }

    public async Task<Result> RemoveProfilePictureAsync(string userId)
    {
        var user = await GetUserByIdAsync(userId);
        
        if (user is null)
            return Result.Failure(Error.User.NotFound());

        if (user.ImagePath is null)
            return Result.Success();
        

        string oldImagePath = user.ImagePath;

        user.ImagePath = null;

        var updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            logger.LogError("Unexpected error happened while deleting profile picture");
            foreach (var error in  updateResult.Errors)
            {
                logger.LogError("ERROR: {err}", error.Description);
            }

            return Result.Failure(Error.Database.QueryError("Došlo je do greške prilikom brisanja profilne slike."));
        }

        if(!fileService.DeleteFile(oldImagePath))
            logger.LogError("User's old profile picture could not be removed from system's files.");
        
        logger.LogInformation("User's profile picture has been removed from the application successfully.");
        return Result.Success();

    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        
        var deleteResult = await userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
        {
            logger.LogError("Unexpected error happened while deleting user from database");
            foreach (var error in deleteResult.Errors)
                logger.LogError("ERROR: {err}", error.Description);
            return Result.Failure(Error.Database.QueryError("Došlo je do greške prilikom brisanja korisnika."));
        }
        
        logger.LogInformation("User has been deleted successfully.");
        return Result.Success();
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await GetUserByIdAsync(userId);

        if (user is null)
        {
            logger.LogError("User not found");
            return Result.Failure(Error.User.NotFound());       
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

    private async Task<Result> AssignRole(ApplicationUser user, string role)
    {
        
        if (!IsValidRole(role))
        {
            logger.LogError("Invalid user role entered");
            return Result.Failure(Error.Validation.InvalidInput("Nevažeća korisnička rola"));
        }
        
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
            
        var roleCreateResult = await userManager.AddToRoleAsync(user, role);

        if (!roleCreateResult.Succeeded)
        {
            logger.LogError("Unexpected error happened while assigning user role");
            foreach (var error in roleCreateResult.Errors)
                logger.LogError("ERROR: {err}", error.Description);
            
            return Result.Failure(Error.Database.QueryError("Došlo je do greške prilikom dodavanja korisnika u rolu."));
        }
        logger.LogInformation("User assigned to role successfully.");
        return Result.Success();
    }

    private async Task<Result> IsUserTaken(string username, string email)
    {
        
        if (await GetUserByEmailAsync(email) is not null)
        {
            logger.LogError("Email address is taken");
            return Result.Failure(Error.User.EmailAlreadyExists());
        }
        
        if (await GetUserByNameAsync(username) is not null)
        {
            logger.LogError("Username is taken");
            return Result.Failure(Error.User.UsernameAlreadyExists(username)); 
        }

        logger.LogInformation("Credentials check passed successfully.");
        return Result.Success();
    }

    private static bool RequiresUserUpdate(ProfileSettingsViewModel model, ApplicationUser user)
    {
        return user.UserName != model.UserName ||
               user.Email != model.Email ||
               user.FirstName != model.FirstName ||
               user.LastName != model.LastName ||
               user.PhoneNumber != model.PhoneNumber ||
               model.ProfilePicture != null;
    }
}