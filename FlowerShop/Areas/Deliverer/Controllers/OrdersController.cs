using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Orders.Commands.UpdateOrderStatus;
using FlowerShop.Application.Features.Orders.Queries.GetDelivererOrderDetails;
using FlowerShop.Application.Features.Orders.Queries.GetDelivererOrders;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Infrastructure.Htmx;
using FlowerShop.Web.Areas.Deliverer.Models.Orders;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Areas.Deliverer.Controllers;

[Area("Deliverer")]
[Authorize(Roles = "Deliverer")]
public class OrdersController(
    IUserProvider userProvider,
    GetDelivererOrdersHandler getDelivererOrdersHandler,
    GetDelivererOrderDetailsHandler getDelivererOrderDetailsHandler,
    UpdateOrderStatusHandler updateOrderStatusHandler) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        string? searchBy = null,
        string? sortBy = "date_desc",
        OrderStatus? status = null,
        DeliveryStatus? deliveryStatus = null,
        int pageIndex = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var delivererId = userProvider.GetCurrentUserId();

        var query = new GetDelivererOrdersQuery
        {
            DelivererId = delivererId,
            SearchBy = searchBy,
            SortBy = sortBy,
            Status = status,
            DeliveryStatus = deliveryStatus,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var summary = await getDelivererOrdersHandler.Handle(query, ct);

        var viewModel = new OrderSummaryViewModel
        {
            PagedOrders = summary.PagedOrders,
            TotalDeliveries = summary.TotalDeliveries,
            ActiveDeliveries = summary.ActiveDeliveries,
            CompletedDeliveries = summary.CompletedDeliveries,
            AverageRating = summary.AverageRating,
            SearchBy = searchBy,
            SortBy = sortBy,
            SelectedStatus = status,
            SelectedDeliveryStatus = deliveryStatus
        };

        if (Request.IsHtmx())
        {
            return PartialView("_OrdersPage", viewModel);
        }

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        string? searchBy = null,
        string? sortBy = "date_desc",
        OrderStatus? status = null,
        DeliveryStatus? deliveryStatus = null,
        int pageIndex = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var delivererId = userProvider.GetCurrentUserId();

        var query = new GetDelivererOrdersQuery
        {
            DelivererId = delivererId,
            SearchBy = searchBy,
            SortBy = sortBy,
            Status = status,
            DeliveryStatus = deliveryStatus,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var summary = await getDelivererOrdersHandler.Handle(query, ct);

        if (Request.IsHtmx())
        {
            return PartialView("_OrdersList", summary.PagedOrders);
        }

        return RedirectToAction(nameof(Index), new { searchBy, sortBy, status, deliveryStatus, pageIndex, pageSize });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct = default)
    {
        var delivererId = userProvider.GetCurrentUserId();

        var result = await getDelivererOrderDetailsHandler.Handle(new GetDelivererOrderDetailsQuery(id, delivererId), ct);
        if (!result.IsSuccess)
        {
            Response.ShowError("Tražena porudžbina nije pronađena ili nemate pravo pristupa.");
            return RedirectToAction(nameof(Index));
        }

        var viewModel = new OrderDetailsViewModel
        {
            Order = result.Payload!
        };

        if (Request.IsHtmx())
        {
            return PartialView("_OrderDetails", viewModel);
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int orderId, DeliveryStatus deliveryStatus, CancellationToken ct = default)
    {
        var delivererId = userProvider.GetCurrentUserId();

        var command = new UpdateOrderStatusCommand
        {
            OrderId = orderId,
            DelivererId = delivererId,
            DeliveryStatus = deliveryStatus
        };

        var result = await updateOrderStatusHandler.Handle(command, ct);
        if (!result.IsSuccess)
        {
            var errorMessage = result.Errors[0].Description ?? "Došlo je do greške prilikom ažuriranja statusa.";
            Response.ShowError(errorMessage);
        }
        else
        {
            Response.ShowSuccess("Status porudžbine je uspešno ažuriran.");
        }

        if (Request.IsHtmx())
        {
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        return RedirectToAction(nameof(Details), new { id = orderId });
    }
}