namespace FlowerShop.Domain.Entities.Carts;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<CartItem?> GetItemByIdForUserAsync(int cartItemId, string userId, CancellationToken ct = default);
    void Add(Cart cart);
    void Update(Cart cart);
    void Remove(Cart cart);
    void RemoveItem(CartItem item);
}
