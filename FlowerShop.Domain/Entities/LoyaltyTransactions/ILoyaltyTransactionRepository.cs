namespace FlowerShop.Domain.Entities.LoyaltyTransactions;

public interface ILoyaltyTransactionRepository
{
    public void Add(LoyaltyTransaction transaction);
    public void Update(LoyaltyTransaction transaction);
    public void Remove(LoyaltyTransaction transaction);
}