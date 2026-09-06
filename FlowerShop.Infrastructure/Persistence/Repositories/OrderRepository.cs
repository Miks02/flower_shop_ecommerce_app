using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Infrastructure.Persistence.EntityFramework;
using FlowerShop.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence.Repositories;

public class OrderRepository(AppDbContext context) : Repository<Order>(context), IOrderRepository
{
    public async Task<Order?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Orders
            .AsSplitQuery()
            .Include(o => o.OrderItems)
            .Include(o => o.User)
            .Include(o => o.Deliverer)
                .ThenInclude(d => d!.User)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<Order?> GetByIdForUserAsync(int id, string userId, CancellationToken ct = default)
    {
        return await context.Orders
            .AsSplitQuery()
            .Include(o => o.OrderItems)
            .Include(o => o.User)
            .Include(o => o.Deliverer)
                .ThenInclude(d => d!.User)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId, ct);
    }

    public async Task<PagedResult<Order>> GetPagedOrdersForUserAsync(
        string userId,
        string? searchBy,
        string? sortBy,
        OrderStatus? status,
        int pageIndex,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId);

        if (!string.IsNullOrWhiteSpace(searchBy))
        {
            var search = searchBy.Trim();
            query = query.Where(o =>
                o.OrderNumber.Contains(search) ||
                o.RecipientFullName.Contains(search) ||
                o.City.Contains(search) ||
                o.OrderAddress.Contains(search) ||
                o.OrderItems.Any(i => i.ProductName.Contains(search)));
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.OrderStatus == status.Value);
        }

        query = sortBy switch
        {
            "date_asc" => query.OrderBy(o => o.CreatedAt),
            "date_desc" => query.OrderByDescending(o => o.CreatedAt),
            "price_asc" => query.OrderBy(o => o.OrderItems.Sum(i => i.Quantity * i.UnitPrice)),
            "price_desc" => query.OrderByDescending(o => o.OrderItems.Sum(i => i.Quantity * i.UnitPrice)),
            "delivery_asc" => query.OrderBy(o => o.OrderDate),
            "delivery_desc" => query.OrderByDescending(o => o.OrderDate),
            _ => query.OrderByDescending(o => o.CreatedAt)
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Order>(items, pageIndex, pageSize, totalCount, items.Count);
    }

    public async Task<(int TotalOrders, int PendingOrders, int InDeliveryOrders, int CompletedOrders)> GetUserOrderStatsAsync(string userId, CancellationToken ct = default)
    {
        var userOrders = context.Orders.Where(o => o.UserId == userId);

        var total = await userOrders.CountAsync(ct);
        var pending = await userOrders.CountAsync(o => o.OrderStatus == OrderStatus.Pending, ct);
        var inDelivery = await userOrders.CountAsync(o => o.DeliveryStatus == DeliveryStatus.InTransit || o.DeliveryStatus == DeliveryStatus.OnTheWay, ct);
        var completed = await userOrders.CountAsync(o => o.OrderStatus == OrderStatus.Completed, ct);

        return (total, pending, inDelivery, completed);
    }

    public OrderItem? GetItemById(int id)
    {
        return context.OrderItems.FirstOrDefault(i => i.Id == id);
    }

    public void RemoveItem(OrderItem orderItem)
    {
        context.OrderItems.Remove(orderItem);
    }
}