using FlowerShop.Domain.Entities.LoyaltyTransactions;
using FlowerShop.Infrastructure.Persistence.EntityFramework;

namespace FlowerShop.Infrastructure.Persistence.Repositories;

public class LoyaltyTransactionRepository(AppDbContext context) : Repository<LoyaltyTransaction>(context), ILoyaltyTransactionRepository
{
    
}