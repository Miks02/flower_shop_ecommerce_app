using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Common.Abstractions.Dto;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.SharedKernel.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FlowerShop.Application.Features.Users.Commands;

public class UpdateProfileHandler(UserManager<User> userManager, ILogger<UpdateProfileHandler> logger, IFileService fileService) : IHandler
{
    public async Task<Result<UserDetailsDto>> Handle(UpdateProfileCommand command)
    {
        var currentUser = await userManager.FindByIdAsync(command.UserId);

        if (currentUser is null)
            return Result<UserDetailsDto>.Failure(UserError.NotFound(command.UserId));


        currentUser.FirstName = command.FirstName;
        currentUser.LastName = command.LastName;
        currentUser.UserName = command.UserName;
        currentUser.Email = command.Email;
        currentUser.PhoneNumber = command.PhoneNumber;

        currentUser.NormalizedEmail = userManager.NormalizeEmail(currentUser.Email);
        currentUser.NormalizedUserName = userManager.NormalizeName(currentUser.UserName);

        if (command.ProfilePicture is not null && command.ProfilePicture.Length > 0)
        {
            var imageUploadResult = await fileService.UploadFile(command.ProfilePicture, currentUser.ImagePath ?? "", "user-images");

            if (!imageUploadResult.IsSuccess)
            {
                logger.LogWarning("Error occurred while trying to update user's profile");
            
                var errors = imageUploadResult.Errors
                    .Select(e => new Error(e.Code, e.Description))
                    .ToList();

                foreach (var error in errors)
                    logger.LogWarning("Code: [{code}] Description: [{description}]", error.Code, error.Description);
                
                return Result<UserDetailsDto>.Failure(errors.ToArray());
            }

            currentUser.ImagePath = imageUploadResult.Payload;
        }

        var updateResult = await userManager.UpdateAsync(currentUser);

        if (!updateResult.Succeeded)
        {
            logger.LogWarning("Error occurred while trying to update user's profile");
            
            var errors = updateResult.Errors
                .Select(e => new Error(e.Code, e.Description))
                .ToList();

            foreach (var error in errors)
                logger.LogWarning("Code: [{code}] Description: [{description}]", error.Code, error.Description);
            
            
            return Result<UserDetailsDto>.Failure(errors.ToArray());
        }

        var userDetailsDto = new UserDetailsDto
        {
            FirstName = currentUser.FirstName,
            LastName = currentUser.LastName,
            Username = currentUser.UserName,
            PhoneNumber = currentUser.PhoneNumber,
            Email = currentUser.Email,
        };
        
        return Result<UserDetailsDto>.Success(userDetailsDto);
    }
}