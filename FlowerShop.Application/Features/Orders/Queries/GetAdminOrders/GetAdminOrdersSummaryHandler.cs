using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Orders.Queries.GetAdminOrders;

public class GetAdminOrdersSummaryHandler(IOrderRepository orderRepo) : IHandler
{
    public async Task<GetAdminOrdersSummaryResponse> Handle(GetAdminOrdersSummaryQuery request, CancellationToken ct = default)
    {
        var pagedOrders = await orderRepo.GetPagedOrdersForAdminAsync(
            request.SearchBy,
            request.SortBy,
            request.Status,
            request.DeliveryStatus,
            request.IsAssigned,
            request.PageIndex,
            request.PageSize,
            ct);

        var stats = await orderRepo.GetAdminOrderStatsAsync(ct);

        var mappedItems = pagedOrders.Items.Select(o =>
        {
            var buyerName = o.User != null ? $"{o.User.FirstName} {o.User.LastName}".Trim() : null;
            var delivererName = o.Deliverer?.User != null ? $"{o.Deliverer.User.FirstName} {o.Deliverer.User.LastName}".Trim() : null;

            return new AdminOrderDto
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
                BuyerFullName = buyerName,
                BuyerEmail = o.User?.Email,
                DelivererId = o.DelivererId,
                DelivererFullName = delivererName,
                Items = o.OrderItems.Select(i => new AdminOrderItemPreviewDto
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    ProductImagePath = i.ProductImagePath,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }).ToList();

        var pagedResult = new PagedResult<AdminOrderDto>(
            mappedItems,
            pagedOrders.Page,
            pagedOrders.PageSize,
            pagedOrders.TotalCount,
            mappedItems.Count);

        return new GetAdminOrdersSummaryResponse
        {
            PagedOrders = pagedResult,
            TotalOrders = stats.TotalOrders,
            PendingOrders = stats.PendingOrders,
            UnassignedOrders = stats.UnassignedOrders,
            InDeliveryOrders = stats.InDeliveryOrders,
            CompletedOrders = stats.CompletedOrders
        };
    }
}
