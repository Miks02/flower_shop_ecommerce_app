using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.SharedKernel.ErrorCatalogue;
using FlowerShop.SharedKernel.Results;
using Microsoft.AspNetCore.Identity;

namespace FlowerShop.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserHandler(UserManager<User> userManager, RoleManager<IdentityRole> roleManager) : IHandler
{
    public async Task<Result> Handle(RegisterUserCommand command)
    {
        var credentialsCheckResult = await IsUserTaken(command.Username, command.Email);

        if (!credentialsCheckResult.IsSuccess)
            return Result.Failure(AuthError.RegistrationFailed(credentialsCheckResult.Errors[0].Description));
        
        var user = new User
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            UserName = command.Username,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,
        };

        var userCreateResult = await userManager.CreateAsync(user, command.Password);

        if (!userCreateResult.Succeeded)
        {
            
            var errors = userCreateResult.Errors
                .Select(x => new Error(x.Code, x.Description))
                .ToList();

            return Result.Failure(errors.ToArray());
        }
        
        var roleAssignResult = await AssignRoleAsync(user);

        if (!roleAssignResult.IsSuccess)
        {
            var errors = roleAssignResult.Errors!;

            await userManager.DeleteAsync(user);
            
            return Result.Failure(errors.ToArray());
        }
        
        return Result.Success();
    }
    
    private async Task<Result> AssignRoleAsync(User user)
    {
        var role = "User";
        
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
            
        var roleCreateResult = await userManager.AddToRoleAsync(user, role);

        if (!roleCreateResult.Succeeded)
        {
            var errors = roleCreateResult.Errors
                .Select(x => new Error(x.Code, x.Description))
                .ToList();
            
            return Result.Failure(errors.ToArray());
        }
        return Result.Success();
    }
    
    private async Task<Result> IsUserTaken(string username, string email)
    {
        if (await userManager.FindByEmailAsync(email) is not null)
            return Result.Failure(UserError.EmailAlreadyExists());
        
        if (await userManager.FindByNameAsync(username) is not null)
            return Result.Failure(UserError.UsernameAlreadyExists(username)); 
        

        return Result.Success();
    }
}