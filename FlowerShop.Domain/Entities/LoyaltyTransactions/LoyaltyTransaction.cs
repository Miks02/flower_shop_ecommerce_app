using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.Domain.Entities.Orders;

namespace FlowerShop.Domain.Entities.LoyaltyTransactions;

public class LoyaltyTransaction
{
    public int Id { get; init; }
    public TransactionType TransactionType { get; init; }
    public int Points { get; init; }
    public DateTime TransactionDate { get; init; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public string UserId { get; set; } = null!;

    public Order Order { get; set; } = null!;
    public int OrderId { get; set; }
}