using System.Security.Claims;
using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Common.Abstractions.Dto;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.Infrastructure.Persistence.EntityFramework;
using FlowerShop.SharedKernel.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Identity;

public class UserProvider(IHttpContextAccessor http, AppDbContext context) : IUserProvider
{
    public string GetCurrentUserId()
    {
        return http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) 
               ?? throw new UnauthorizedAccessException();
    }

    public async Task<UserDetailsDto> GetCurrentUserDetails(string userId, CancellationToken ct = default)
    {
        var user = await context.Users
            .Where(x => x.Id == userId)
            .Select(e => new UserDetailsDto
            {
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email!,
                Username = e.UserName!,
                PhoneNumber = e.PhoneNumber!,
                Address = "",
                ProfilePicture = e.ImagePath,
                RegistrationDate = e.CreatedAt
            })
            .FirstOrDefaultAsync(ct);

        if (user is null)
            throw new KeyNotFoundException("User not found");

        return user;
    }
}