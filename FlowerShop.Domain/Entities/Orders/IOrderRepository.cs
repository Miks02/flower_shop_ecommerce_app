using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Orders;

public interface IOrderRepository
{
    void Add(Order order);
    void Update(Order order);
    void Remove(Order order);
    Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Order?> GetByIdForUserAsync(int id, string userId, CancellationToken ct = default);
    Task<PagedResult<Order>> GetPagedOrdersForUserAsync(
        string userId,
        string? searchBy,
        string? sortBy,
        OrderStatus? status,
        int pageIndex,
        int pageSize,
        CancellationToken ct = default);
    Task<(int TotalOrders, int PendingOrders, int InDeliveryOrders, int CompletedOrders)> GetUserOrderStatsAsync(string userId, CancellationToken ct = default);
    Task<PagedResult<Order>> GetPagedOrdersForAdminAsync(
        string? searchBy,
        string? sortBy,
        OrderStatus? status,
        DeliveryStatus? deliveryStatus,
        bool? isAssigned,
        int pageIndex,
        int pageSize,
        CancellationToken ct = default);

    Task<PagedResult<Order>> GetPagedOrdersForDeliverersAsync(
        string delivererId,
        string? searchBy,
        string? sortBy,
        OrderStatus? status,
        DeliveryStatus? deliveryStatus,
        int pageIndex,
        int pageSize,
        CancellationToken ct = default);
    Task<(int TotalOrders, int PendingOrders, int UnassignedOrders, int InDeliveryOrders, int CompletedOrders)>
        GetAdminOrderStatsAsync(CancellationToken ct = default);
    Task<(int TotalDeliveries, int ActiveDeliveries, int CompletedDeliveries, decimal AverageRating)> 
        GetDelivererOrderStatsAsync(string delivererId, CancellationToken ct = default);
}