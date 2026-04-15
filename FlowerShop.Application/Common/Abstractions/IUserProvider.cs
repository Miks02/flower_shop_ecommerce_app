using FlowerShop.Application.Common.Abstractions.Dto;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Common.Abstractions;

public interface IUserProvider
{
    string GetCurrentUserId();
    Task<UserDetailsDto> GetCurrentUserDetails(string userId, CancellationToken ct = default);
}