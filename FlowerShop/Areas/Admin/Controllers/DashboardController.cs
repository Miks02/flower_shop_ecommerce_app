using FlowerShop.Application.Features.Dashboard.Queries.GetAdminDashboard;
using FlowerShop.Web.Areas.Admin.Models.Dashboard;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController(GetAdminDashboardHandler getAdminDashboardHandler) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var data = await getAdminDashboardHandler.Handle(ct);

        var viewModel = new AdminDashboardViewModel
        {
            TodaySales = data.TodaySales,
            TodayNewOrders = data.TodayNewOrders,
            ActiveOrders = data.ActiveOrders,
            AvailableProducts = data.AvailableProducts,
            DeliverersOnDuty = data.DeliverersOnDuty,
            RegisteredCustomers = data.RegisteredCustomers,
            RecentOrders = data.RecentOrders.Select(o => new AdminRecentOrderViewModel
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerFullName = o.CustomerFullName,
                OrderPrice = o.OrderPrice,
                OrderStatus = o.OrderStatus,
                CreatedAt = o.CreatedAt
            }).ToList()
        };

        if (Request.IsHtmx())
            return PartialView("_DashboardPartial", viewModel);

        return View(viewModel);
    }
}
