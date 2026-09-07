using FlowerShop.Domain.Entities.LoyaltyTransactions;
using FlowerShop.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence.Repositories;

public class LoyaltyTransactionRepository(AppDbContext context) : Repository<LoyaltyTransaction>(context), ILoyaltyTransactionRepository
{
    public async Task<LoyaltyTransaction?> GetMostRecentLoyaltyTransaction(string userId, CancellationToken ct = default)
    {
        return await context.LoyaltyTransactions
            .Where(lt => lt.UserId == userId)
            .OrderByDescending(lt => lt.TransactionDate)
            .FirstOrDefaultAsync(ct); 
    }

    public async Task<int> GetCurrentLoyaltyPoints(string userId, CancellationToken ct = default)
    {
        return await context.LoyaltyTransactions
            .Where(lt => lt.UserId == userId)
            .OrderByDescending(lt => lt.TransactionDate)
            .Select(lt => lt.Points)
            .FirstOrDefaultAsync(ct);
    }
}