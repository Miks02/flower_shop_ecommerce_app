using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Deliverers.Queries.GetDelivererDetails;

using FlowerShop.Web.Areas.Deliverer.Models;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Areas.Deliverer.Controllers;

[Area("Deliverer")]
[Authorize(Roles = "Deliverer")]
public class DashboardController(
    IUserProvider userProvider,
    GetDelivererDetailsHandler getDelivererHandler) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var userId = userProvider.GetCurrentUserId();
        
        var detailsResult = await getDelivererHandler.Handle(new GetDelivererDetailsQuery(userId), ct);
        
        if (!detailsResult.IsSuccess)
            return NotFound();

        var data = detailsResult.Payload!;
        
        var viewModel = new DelivererDashboardViewModel
        {
            DelivererId = data.DelivererId,
            FirstName = data.FirstName,
            LastName = data.LastName,
            Email = data.Email,
            PhoneNumber = data.PhoneNumber,
            ProfilePicture = data.ProfilePicture,
            AccountStatus = data.AccountStatus,
            DelivererStatus = data.DelivererStatus,
            VehicleType = data.VehicleType,
            RegistrationDate = data.RegistrationDate,
            AverageRating = data.AverageRating,
            TotalDeliveries = data.TotalDeliveries,
            ActiveDeliveries = data.ActiveDeliveries,
            CompletedDeliveries = data.CompletedDeliveries
        };

        if (Request.IsHtmx())
        {
            return PartialView("_DashboardContent", viewModel);
        }

        return View(viewModel);
    }
}