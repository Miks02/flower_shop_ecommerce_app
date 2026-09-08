using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Orders.Commands.CreateOrder;
using FlowerShop.Application.Features.Orders.Queries.GetOrderReceipt;
using FlowerShop.Application.Features.Orders.Queries.GetUserOrders;
using FlowerShop.Application.Features.Reviews.Commands.CreateReview;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Infrastructure.Htmx;
using FlowerShop.Web.Areas.User.Models.Orders;
using FlowerShop.Web.Controllers;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = "User")]
public class OrdersController(
    IUserProvider userProvider,
    GetUserOrdersSummaryHandler getUserOrdersSummaryHandler,
    GetOrderReceiptHandler getOrderReceiptHandler,
    CreateReviewHandler createReviewHandler,
    ILogger<OrdersController> logger) : BaseController(logger)
{
    [HttpGet]
    public async Task<IActionResult> Index(
        int? orderId = null,
        string? searchBy = null,
        string? sortBy = "date_desc",
        OrderStatus? status = null,
        int pageIndex = 1,
        CancellationToken ct = default)
    {
        var userId = userProvider.GetCurrentUserId();

        var query = new GetUserOrdersSummaryQuery
        {
            UserId = userId,
            SearchBy = searchBy,
            SortBy = sortBy,
            Status = status,
            PageIndex = pageIndex,
            PageSize = 8
        };

        var summary = await getUserOrdersSummaryHandler.Handle(query, ct);

        var viewModel = new UserOrdersSummaryViewModel
        {
            PagedOrders = summary.PagedOrders,
            TotalOrders = summary.TotalOrders,
            PendingOrders = summary.PendingOrders,
            InDeliveryOrders = summary.InDeliveryOrders,
            CompletedOrders = summary.CompletedOrders,
            SearchBy = searchBy,
            SortBy = sortBy,
            SelectedStatus = status
        };

        ViewBag.InitialOrderId = orderId;

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
        int pageIndex = 1,
        CancellationToken ct = default)
    {
        var userId = userProvider.GetCurrentUserId();

        var query = new GetUserOrdersSummaryQuery
        {
            UserId = userId,
            SearchBy = searchBy,
            SortBy = sortBy,
            Status = status,
            PageIndex = pageIndex,
            PageSize = 8
        };

        var summary = await getUserOrdersSummaryHandler.Handle(query, ct);

        if (Request.IsHtmx())
        {
            return PartialView("_OrdersList", summary.PagedOrders);
        }

        return RedirectToAction(nameof(Index), new { searchBy, sortBy, status, pageIndex });
    }

    [HttpGet]
    [Route("/User/Orders/Receipt/{id:int}")]
    public async Task<IActionResult> ReceiptModal(int id, CancellationToken ct = default)
    {
        var userId = userProvider.GetCurrentUserId();
        var result = await getOrderReceiptHandler.Handle(new GetOrderReceiptQuery(id, userId), ct);

        if (!result.IsSuccess)
        {
            Response.ShowError("Tražena porudžbina nije pronađena.");
            return NoContent();
        }

        return PartialView("_OrderReceiptModal", result.Payload);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/User/Orders/Review/{id:int}")]
    public async Task<IActionResult> SubmitReview(int id, [FromForm] SubmitOrderReviewRequest request, CancellationToken ct = default)
    {
        var userId = userProvider.GetCurrentUserId();

        var command = new CreateReviewCommand(userId, id, request.Rating, request.Comment);

        if (!ModelState.IsValid)
        {
            var unReviewedReceipt = await getOrderReceiptHandler.Handle(new GetOrderReceiptQuery(id, userId), ct);
            Response.ShowWarning("Proverite sva polja pre slanja recenzije.");
            return PartialView("_OrderReceiptModal", unReviewedReceipt.Payload);
        }
        
        var result = await createReviewHandler.Handle(command, ct);


        if (!result.IsSuccess)
        {
            Response.ShowError(result.Errors[0].Description);
            return NoContent();
        }

        var receipt = await getOrderReceiptHandler.Handle(new GetOrderReceiptQuery(id, userId), ct);

        if (!receipt.IsSuccess)
        {
            Response.ShowError("Tražena porudžbina nije pronađena.");
            return NoContent();
        }

        Response.ShowSuccess("Hvala vam na ostavljenoj recenziji. Vaši utisci su nam od velikog značaja!");
        return PartialView("_OrderReceiptModal", receipt.Payload);
    }
}
