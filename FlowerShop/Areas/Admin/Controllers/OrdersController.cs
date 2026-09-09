using FlowerShop.Application.Features.Orders.Commands.AssignOrder;
using FlowerShop.Application.Features.Orders.Queries.GetAdminOrderDetails;
using FlowerShop.Application.Features.Orders.Queries.GetAdminOrders;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Infrastructure.Htmx;
using FlowerShop.Web.Areas.Admin.Models.Orders;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OrdersController(
    GetAdminOrdersSummaryHandler getAdminOrdersSummaryHandler,
    GetAdminOrderDetailsHandler getAdminOrderDetailsHandler,
    AssignOrderHandler assignOrderHandler) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        string? searchBy = null,
        string? sortBy = "date_desc",
        OrderStatus? status = null,
        DeliveryStatus? deliveryStatus = null,
        bool? isAssigned = null,
        int pageIndex = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetAdminOrdersSummaryQuery
        {
            SearchBy = searchBy,
            SortBy = sortBy,
            Status = status,
            DeliveryStatus = deliveryStatus,
            IsAssigned = isAssigned,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var summary = await getAdminOrdersSummaryHandler.Handle(query, ct);

        var viewModel = new OrderSummaryViewModel
        {
            PagedOrders = summary.PagedOrders,
            TotalOrders = summary.TotalOrders,
            PendingOrders = summary.PendingOrders,
            UnassignedOrders = summary.UnassignedOrders,
            InDeliveryOrders = summary.InDeliveryOrders,
            CompletedOrders = summary.CompletedOrders,
            SearchBy = searchBy,
            SortBy = sortBy,
            SelectedStatus = status,
            SelectedDeliveryStatus = deliveryStatus,
            IsAssigned = isAssigned
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
        bool? isAssigned = null,
        int pageIndex = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetAdminOrdersSummaryQuery
        {
            SearchBy = searchBy,
            SortBy = sortBy,
            Status = status,
            DeliveryStatus = deliveryStatus,
            IsAssigned = isAssigned,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var summary = await getAdminOrdersSummaryHandler.Handle(query, ct);

        if (Request.IsHtmx())
        {
            return PartialView("_OrdersList", summary.PagedOrders);
        }

        return RedirectToAction(nameof(Index), new { searchBy, sortBy, status, deliveryStatus, isAssigned, pageIndex, pageSize });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct = default)
    {
        var result = await getAdminOrderDetailsHandler.Handle(new GetAdminOrderDetailsQuery(id), ct);
        if (!result.IsSuccess)
        {
            Response.ShowError("Tražena porudžbina nije pronađena.");
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
    public async Task<IActionResult> Assign(int orderId, string delivererId, CancellationToken ct = default)
    {
        var command = new AssignOrderCommand
        {
            OrderId = orderId,
            DelivererId = delivererId
        };

        var result = await assignOrderHandler.Handle(command, ct);
        if (!result.IsSuccess)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Description ?? "Došlo je do greške prilikom dodele dostavljača.";
            Response.ShowError(errorMessage);
        }
        else
        {
            Response.ShowSuccess("Dostavljač je uspešno dodeljen porudžbini.");
        }

        return await Details(orderId, ct);
    }
}