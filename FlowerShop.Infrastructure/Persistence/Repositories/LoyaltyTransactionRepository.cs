using FlowerShop.Domain.Entities.LoyaltyTransactions;
using FlowerShop.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence.Repositories;

public class LoyaltyTransactionRepository(AppDbContext context) : Repository<LoyaltyTransaction>(context), ILoyaltyTransactionRepository
{
    public async Task<LoyaltyTransaction?> GetMostRecentLoyaltyTransaction(string userId, CancellationToken ct = default)
    {
        return await Context.LoyaltyTransactions
            .AsNoTracking()
            .Where(lt => lt.UserId == userId)
            .OrderByDescending(lt => lt.TransactionDate)
            .FirstOrDefaultAsync(ct); 
    }

    public async Task<int> GetCurrentLoyaltyPoints(string userId, CancellationToken ct = default)
    {
        return await Context.LoyaltyTransactions
            .AsNoTracking()
            .Where(lt => lt.UserId == userId)
            .OrderByDescending(lt => lt.TransactionDate)
            .Select(lt => lt.CurrentPoints)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<LoyaltyTransaction?> GetLastLoyaltyTransactionByOrderId(int orderId, CancellationToken ct = default)
    {
        return await Context.LoyaltyTransactions
            .AsNoTracking()
            .Where(lt => lt.OrderId == orderId)
            .OrderByDescending(lt => lt.TransactionDate)
            .FirstOrDefaultAsync(ct);
    }
    
    public async Task<int> GetAllSpentLoyaltyPoints(CancellationToken ct = default)
    {
        return await Context.LoyaltyTransactions
            .Where(lt => lt.TransactionType == TransactionType.Redeemed)
            .SumAsync(lt => lt.PreviousPoints, ct);
    }

    public async Task<int> GetAllSpentLoyaltyPointsByUserId(string userId, CancellationToken ct = default)
    {
        return await Context.LoyaltyTransactions
            .Where(lt => lt.TransactionType == TransactionType.Redeemed && lt.UserId == userId)
            .SumAsync(lt => lt.PreviousPoints, ct);
    }
}