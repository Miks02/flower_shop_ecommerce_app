using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Orders.Queries.GetUserOrders;

public class GetUserOrdersSummaryHandler(IOrderRepository orderRepo) : IHandler
{
    public async Task<UserOrdersSummaryResponse> Handle(GetUserOrdersSummaryQuery request, CancellationToken ct = default)
    {
        var pagedOrders = await orderRepo.GetPagedOrdersForUserAsync(
            request.UserId,
            request.SearchBy,
            request.SortBy,
            request.Status,
            request.PageIndex,
            request.PageSize,
            ct);

        var stats = await orderRepo.GetUserOrderStatsAsync(request.UserId, ct);

        var mappedItems = pagedOrders.Items.Select(o => new UserOrderDto
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            RecipientFullName = o.RecipientFullName,
            RecipientPhoneNumber = o.RecipientPhoneNumber,
            OrderAddress = o.OrderAddress,
            City = o.City,
            ZipCode = o.ZipCode,
            OrderStatus = o.OrderStatus,
            DeliveryStatus = o.DeliveryStatus,
            OrderDate = o.OrderDate,
            CreatedAt = o.CreatedAt,
            Note = o.Note,
            OrderPrice = o.OrderPrice,
            TotalItemsCount = o.OrderItems.Sum(i => i.Quantity),
            IsRated = o.Review != null,
            Items = o.OrderItems.Select(i => new UserOrderItemDto
            {
                Id = i.Id,
                ProductName = i.ProductName,
                ProductImagePath = i.ProductImagePath,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        }).ToList();

        var pagedResult = new PagedResult<UserOrderDto>(
            mappedItems,
            pagedOrders.Page,
            pagedOrders.PageSize,
            pagedOrders.TotalCount,
            mappedItems.Count);

        return new UserOrdersSummaryResponse
        {
            PagedOrders = pagedResult,
            TotalOrders = stats.TotalOrders,
            PendingOrders = stats.PendingOrders,
            InDeliveryOrders = stats.InDeliveryOrders,
            CompletedOrders = stats.CompletedOrders
        };
    }
}
