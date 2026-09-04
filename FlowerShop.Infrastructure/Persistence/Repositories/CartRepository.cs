using FlowerShop.Domain.Entities.Carts;
using FlowerShop.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence.Repositories;

public class CartRepository : Repository<Cart>, ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _context.Carts
            .AsSplitQuery()
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);
    }

    public async Task<CartItem?> GetItemByIdForUserAsync(int cartItemId, string userId, CancellationToken ct = default)
    {
        return await _context.CartItems
            .Include(i => i.Cart)
            .FirstOrDefaultAsync(i => i.Id == cartItemId && i.Cart.UserId == userId, ct);
    }

    public void RemoveItem(CartItem item)
    {
        _context.CartItems.Remove(item);
    }
}
