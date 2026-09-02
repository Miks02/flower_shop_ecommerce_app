using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.SharedKernel.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FlowerShop.Application.Features.Deliverers.Commands.RegisterDeliverer;

public class RegisterDelivererHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IDelivererRepository delivererRepo,
    ILogger<RegisterDelivererHandler> logger
    ) : IHandler
{
    public async Task<Result> Handle(RegisterDelivererCommand command, CancellationToken ct = default)
    {
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var newUser = new User
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.Email,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
            };

            var password = PasswordGenerator(command.FirstName, command.LastName);

            var creationResult = await userManager.CreateAsync(newUser, password);

            if (!creationResult.Succeeded)
            {
                var errors = creationResult.Errors
                    .Select(x => new Error(x.Code, x.Description))
                    .ToArray();

                return Result.Failure(errors);
            }
            
            var roleAssignResult = await userManager.AddToRoleAsync(newUser, "Deliverer");
            
            if (!roleAssignResult.Succeeded)
            {
                var errors = roleAssignResult.Errors
                    .Select(x => new Error(x.Code, x.Description))
                    .ToArray();

                return Result.Failure(errors);
            }
            
            var deliverer = new Deliverer
            {
                Id = newUser.Id,
                VehicleType = command.VehicleType,
            };
            
            delivererRepo.Add(deliverer);

            await unitOfWork.SaveAsync(ct);
            await unitOfWork.CommitAsync(ct);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync(ct);
            logger.LogError(ex, "An error occurred while registering a new deliverer.");
            throw;
        }
        
    }

    private string PasswordGenerator(string firstName, string lastName)
    {
        return $"{firstName.ToLower()}{lastName.ToLower()}123";
    }
}