using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Application.Features.Orders.Queries.GetDelivererOrders;

public class GetDelivererOrdersHandler(IOrderRepository orderRepo) : IHandler
{
    public async Task<GetDelivererOrdersResponse> Handle(GetDelivererOrdersQuery request, CancellationToken ct = default)
    {
        var pagedOrders = await orderRepo.GetPagedOrdersForDeliverersAsync(
            request.DelivererId,
            request.SearchBy,
            request.SortBy,
            request.Status,
            request.DeliveryStatus,
            request.PageIndex,
            request.PageSize,
            ct);

        var stats = await orderRepo.GetAdminOrderStatsAsync(ct);

        var mappedItems = pagedOrders.Items.Select(o =>
        {
            var buyerName = $"{o.User.FirstName} {o.User.LastName}".Trim();
            var delivererName = o.Deliverer?.User != null ? $"{o.Deliverer.User.FirstName} {o.Deliverer.User.LastName}".Trim() : null;

            return new DelivererOrderDto
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
                IsRated = o.ServiceReview != null,
                BuyerFullName = buyerName,
                BuyerEmail = o.User?.Email,
                DelivererId = o.DelivererId,
                DelivererFullName = delivererName,
                Items = o.OrderItems.Select(i => new DelivererOrderItemPreviewDto
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    ProductImagePath = i.ProductImagePath,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }).ToList();

        var pagedResult = new PagedResult<DelivererOrderDto>(
            mappedItems,
            pagedOrders.Page,
            pagedOrders.PageSize,
            pagedOrders.TotalCount,
            mappedItems.Count);

        return new GetDelivererOrdersResponse
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
