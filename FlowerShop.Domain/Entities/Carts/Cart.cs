using FlowerShop.Domain.Entities.IdentityUser;

namespace FlowerShop.Domain.Entities.Carts;

public class Cart
{
    public const int MaxDistinctItems = 15;

    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<CartItem> Items { get; set; } = [];

    public bool IsLimitReached => Items.Count >= MaxDistinctItems;
}