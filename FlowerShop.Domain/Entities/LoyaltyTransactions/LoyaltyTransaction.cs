using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.Domain.Entities.Orders;

namespace FlowerShop.Domain.Entities.LoyaltyTransactions;

public class LoyaltyTransaction
{
    public int Id { get; set; }
    public TransactionType TransactionType { get; set; }
    public int CurrentPoints { get; set; }
    public int PreviousPoints { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public string UserId { get; set; } = null!;

    public Order Order { get; set; } = null!;
    public int OrderId { get; set; }
}