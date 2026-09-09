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
            .Include(o => o.LoyaltyTransactions)
            .Include(o => o.OrderItems)
            .Include(o => o.User)
            .Include(o => o.ServiceReview)
            .Include(o => o.Deliverer)
                .ThenInclude(d => d!.User)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<Order?> GetByIdForUserAsync(int id, string userId, CancellationToken ct = default)
    {
        return await context.Orders
            .AsSplitQuery()
            .Include(o => o.LoyaltyTransactions)
            .Include(o => o.OrderItems)
            .Include(o => o.User)
            .Include(o => o.ServiceReview)
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
            .AsSplitQuery()
            .Include(o => o.LoyaltyTransactions)
            .Include(o => o.OrderItems)
            .Include(o => o.ServiceReview)
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
        var inDelivery = await userOrders.CountAsync(o => o.DeliveryStatus == DeliveryStatus.InTransit || o.DeliveryStatus == DeliveryStatus.AlmostOnDestination, ct);
        var completed = await userOrders.CountAsync(o => o.OrderStatus == OrderStatus.Completed, ct);

        return (total, pending, inDelivery, completed);
    }

    public async Task<PagedResult<Order>> GetPagedOrdersForAdminAsync(
        string? searchBy,
        string? sortBy,
        OrderStatus? status,
        DeliveryStatus? deliveryStatus,
        bool? isAssigned,
        int pageIndex,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = context.Orders
            .AsNoTracking()
            .AsSplitQuery()
            .Include(o => o.LoyaltyTransactions)
            .Include(o => o.OrderItems)
            .Include(o => o.User)
            .Include(o => o.ServiceReview)
            .Include(o => o.Deliverer)
                .ThenInclude(d => d!.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchBy))
        {
            var search = searchBy.Trim();
            query = query.Where(o =>
                o.OrderNumber.Contains(search) ||
                o.RecipientFullName.Contains(search) ||
                o.City.Contains(search) ||
                o.OrderAddress.Contains(search) ||
                ((o.User.FirstName.Contains(search) || o.User.LastName.Contains(search) || (o.User.Email != null && o.User.Email.Contains(search)))) ||
                (o.Deliverer != null && (o.Deliverer.User.FirstName.Contains(search) || o.Deliverer.User.LastName.Contains(search))) ||
                o.OrderItems.Any(i => i.ProductName.Contains(search)));
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.OrderStatus == status.Value);
        }

        if (deliveryStatus.HasValue)
        {
            query = query.Where(o => o.DeliveryStatus == deliveryStatus.Value);
        }

        if (isAssigned.HasValue)
        {
            query = isAssigned.Value
                ? query.Where(o => o.DelivererId != null)
                : query.Where(o => o.DelivererId == null);
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
    
    public async Task<PagedResult<Order>> GetPagedOrdersForDeliverersAsync(
        string delivererId,
        string? searchBy,
        string? sortBy,
        OrderStatus? status,
        DeliveryStatus? deliveryStatus,
        int pageIndex,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = context.Orders
            .AsNoTracking()
            .AsSplitQuery()
            .Include(o => o.LoyaltyTransactions)
            .Include(o => o.OrderItems)
            .Include(o => o.User)
            .Include(o => o.ServiceReview)
            .Include(o => o.Deliverer)
                .ThenInclude(d => d!.User)
            .Where(o => o.DelivererId == delivererId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchBy))
        {
            var search = searchBy.Trim();
            query = query.Where(o =>
                o.OrderNumber.Contains(search) ||
                o.RecipientFullName.Contains(search) ||
                o.City.Contains(search) ||
                o.OrderAddress.Contains(search) ||
                ((o.User.FirstName.Contains(search) || o.User.LastName.Contains(search) || (o.User.Email != null && o.User.Email.Contains(search)))) ||
                (o.Deliverer != null && (o.Deliverer.User.FirstName.Contains(search) || o.Deliverer.User.LastName.Contains(search))) ||
                o.OrderItems.Any(i => i.ProductName.Contains(search)));
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.OrderStatus == status.Value);
        }

        if (deliveryStatus.HasValue)
        {
            query = query.Where(o => o.DeliveryStatus == deliveryStatus.Value);
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

    public async Task<(int TotalOrders, int PendingOrders, int UnassignedOrders, int InDeliveryOrders, int CompletedOrders, int ActiveOrders)> GetAdminOrderStatsAsync(CancellationToken ct = default)
    {
        var orders = context.Orders.AsNoTracking();

        var total = await orders.CountAsync(ct);
        var pending = await orders.CountAsync(o => o.OrderStatus == OrderStatus.Pending, ct);
        var unassigned = await orders.CountAsync(o => o.DelivererId == null && o.OrderStatus != OrderStatus.Cancelled && o.OrderStatus != OrderStatus.Completed, ct);
        var inDelivery = await orders.CountAsync(o => o.DeliveryStatus == DeliveryStatus.InTransit || o.DeliveryStatus == DeliveryStatus.AlmostOnDestination, ct);
        var completed = await orders.CountAsync(o => o.OrderStatus == OrderStatus.Completed, ct);
        var active = await orders.CountAsync(o => o.OrderStatus != OrderStatus.Completed && o.OrderStatus != OrderStatus.Cancelled, ct);

        return (total, pending, unassigned, inDelivery, completed, active);
    }

    public async Task<(decimal TotalSales, int NewOrdersCount)> GetTodaySalesStatsAsync(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;

        var todaysOrders = context.Orders
            .AsNoTracking()
            .Where(o => o.CreatedAt.Date == today && o.OrderStatus != OrderStatus.Cancelled);

        var totalSales = await todaysOrders.SumAsync(o => (decimal?)o.OrderPrice, ct) ?? 0m;
        var newOrdersCount = await todaysOrders.CountAsync(ct);

        return (totalSales, newOrdersCount);
    }

    public async Task<IReadOnlyList<Order>> GetRecentOrdersAsync(int count, CancellationToken ct = default)
    {
        return await context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .OrderByDescending(o => o.CreatedAt)
            .Take(count)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Order>> GetRecentOrdersForUserAsync(string userId, int count, CancellationToken ct = default)
    {
        return await context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Take(count)
            .ToListAsync(ct);
    }

    public async Task<(int TotalDeliveries, int ActiveDeliveries, int CompletedDeliveries, decimal AverageRating)> GetDelivererOrderStatsAsync(string delivererId, CancellationToken ct = default)
    {
        var orders = context.Orders.AsNoTracking().Where(o => o.DelivererId == delivererId);

        var total = await orders.CountAsync(ct);
        var active = await orders.CountAsync(o => o.OrderStatus != OrderStatus.Completed && o.OrderStatus != OrderStatus.Cancelled, ct);
        var completed = await orders.CountAsync(o => o.OrderStatus == OrderStatus.Completed, ct);
        var ratings= await orders.Where(o => o.ServiceReview != null)
                                        .Select(o => o.ServiceReview!.Rating)
                                        .ToListAsync(ct);
        
        var averageRating = ratings.Count > 0 
            ? ratings.Average() 
            : 0m;
        
        return (total, active, completed, averageRating);
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