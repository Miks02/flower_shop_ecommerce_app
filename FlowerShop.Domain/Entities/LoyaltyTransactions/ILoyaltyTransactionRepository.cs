namespace FlowerShop.Domain.Entities.LoyaltyTransactions;

public interface ILoyaltyTransactionRepository
{
    public void Add(LoyaltyTransaction transaction);
    public void Update(LoyaltyTransaction transaction);
    public void Remove(LoyaltyTransaction transaction);
    Task<LoyaltyTransaction?> GetMostRecentLoyaltyTransaction(string userId, CancellationToken ct = default);
    Task<LoyaltyTransaction?> GetLastLoyaltyTransactionByOrderId(int orderId, CancellationToken ct = default);
    Task<int> GetCurrentLoyaltyPoints(string userId, CancellationToken ct = default);
}