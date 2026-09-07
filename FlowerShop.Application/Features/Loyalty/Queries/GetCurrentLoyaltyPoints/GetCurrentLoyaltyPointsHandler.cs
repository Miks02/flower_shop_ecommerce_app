using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.LoyaltyTransactions;

namespace FlowerShop.Application.Features.Loyalty.Queries.GetCurrentLoyaltyPoints;

public class GetCurrentLoyaltyPointsHandler(ILoyaltyTransactionRepository loyaltyRepo) : IHandler
{
    public async Task<int> Handle(GetCurrentLoyaltyPointsQuery request, CancellationToken ct = default)
    {
        return await loyaltyRepo.GetCurrentLoyaltyPoints(request.UserId, ct);
    }   
}