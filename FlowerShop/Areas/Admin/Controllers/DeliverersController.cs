using FlowerShop.Application.Features.Deliverers.Commands;
using FlowerShop.Application.Features.Deliverers.Commands.DeleteDeliverer;
using FlowerShop.Application.Features.Deliverers.Commands.RegisterDeliverer;
using FlowerShop.Application.Features.Deliverers.Commands.UpdateDeliverer;
using FlowerShop.Application.Features.Deliverers.Commands.UpdateDelivererStatus;
using FlowerShop.Application.Features.Deliverers.Queries.GetDelivererById;
using FlowerShop.Application.Features.Deliverers.Queries.GetDeliverers;
using FlowerShop.Application.Features.Deliverers.Queries.GetDeliverersSummary;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Infrastructure.Htmx;
using FlowerShop.SharedKernel.Results;
using FlowerShop.Web.Areas.Admin.Models.Deliverers;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DeliverersController(
    GetDeliverersHandler getDeliverersHandler,
    GetDelivererByIdHandler getDelivererByIdHandler,
    GetDeliverersSummaryHandler getSummaryHandler,
    RegisterDelivererHandler registerDelivererHandler,
    UpdateDelivererHandler updateDelivererHandler,
    DeleteDelivererHandler deleteDelivererHandler,
    UpdateDelivererStatusHandler updateStatusHandler
    ) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        string search = "",
        string sortBy = "",
        VehicleType? vehicleType = null,
        DelivererStatus? delivererStatus = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetDeliverersSummaryQuery
        {
            Search = search,
            SortBy = sortBy,
            VehicleType = vehicleType,
            DelivererStatus = delivererStatus,
            Page = page,
            PageSize = pageSize
        };

        var summary = await getSummaryHandler.Handle(query, ct);

        var pagedDeliverersListVm = summary.PagedDeliverers.Items
            .Select(d => new DelivererListViewModel
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Email = d.Email,
                PhoneNumber = d.PhoneNumber,
                VehicleType = d.VehicleType,
                DelivererStatus = d.DelivererStatus,
                CreatedAt = d.CreatedAt
            }).ToList();

        var vm = new PagedResult<DelivererListViewModel>(
            pagedDeliverersListVm,
            query.Page,
            query.PageSize,
            summary.PagedDeliverers.TotalCount,
            pagedDeliverersListVm.Count);

        var summaryVm = new DelivererSummaryViewModel
        {
            PagedDeliverers = vm,
            TotalCount = summary.Statistics.TotalCount,
            AvailableCount = summary.Statistics.AvailableCount,
            OnDutyCount = summary.Statistics.OnDutyCount,
            UnavailableCount = summary.Statistics.UnavailableCount,
            BicycleCount = summary.Statistics.BicycleCount,
            ScooterCount = summary.Statistics.ScooterCount,
            CarCount = summary.Statistics.CarCount
        };

        if (Request.IsHtmx())
            return PartialView("_DeliverersPage", summaryVm);

        return View(summaryVm);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        string search = "",
        string sortBy = "",
        VehicleType? vehicleType = null,
        DelivererStatus? delivererStatus = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetDeliverersQuery
        {
            Search = search,
            SortBy = sortBy,
            VehicleType = vehicleType,
            DelivererStatus = delivererStatus,
            Page = page,
            PageSize = pageSize
        };

        var pagedDeliverers = await getDeliverersHandler.Handle(query, ct);

        var pagedDeliverersListVm = pagedDeliverers.Items
            .Select(d => new DelivererListViewModel
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Email = d.Email,
                PhoneNumber = d.PhoneNumber,
                VehicleType = d.VehicleType,
                DelivererStatus = d.DelivererStatus,
                CreatedAt = d.CreatedAt
            }).ToList();

        var vm = new PagedResult<DelivererListViewModel>(
            pagedDeliverersListVm,
            query.Page,
            query.PageSize,
            pagedDeliverers.TotalCount,
            pagedDeliverersListVm.Count);

        if (Request.IsHtmx())
            return PartialView("_DeliverersList", vm);

        return RedirectToAction("Index", vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var vm = new DelivererFormViewModel();

        if (Request.IsHtmx())
            return PartialView("_DeliverersForm", vm);

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DelivererFormViewModel request, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_DeliverersForm", request);
        }

        var command = new RegisterDelivererCommand
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            VehicleType = request.VehicleType
        };

        var result = await registerDelivererHandler.Handle(command, ct);

        if (!result.IsSuccess)
        {
            foreach (var error in result.Errors)
            {
                if (error.Code.Contains("Email", StringComparison.OrdinalIgnoreCase) || error.Code.Contains("DuplicateUserName", StringComparison.OrdinalIgnoreCase))
                    ModelState.AddModelError("Email", error.Description);
                else
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return PartialView("_DeliverersForm", request);
        }

        Response.ShowSuccess("Dostavljač je uspešno registrovan");
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id, CancellationToken ct = default)
    {
        var delivererResult = await getDelivererByIdHandler.Handle(id, ct);
        if (!delivererResult.IsSuccess)
        {
            Response.ShowError("Traženi dostavljač nije pronađen");
            return RedirectToAction("Index");
        }

        var deliverer = delivererResult.Payload!;

        var vm = new DelivererFormViewModel
        {
            Id = deliverer.Id,
            FirstName = deliverer.FirstName,
            LastName = deliverer.LastName,
            Email = deliverer.Email,
            PhoneNumber = deliverer.PhoneNumber,
            VehicleType = deliverer.VehicleType,
            DelivererStatus = deliverer.DelivererStatus,
            CreatedAt = deliverer.CreatedAt
        };

        if (Request.IsHtmx())
            return PartialView("_DeliverersForm", vm);

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DelivererFormViewModel request, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_DeliverersForm", request);
        }

        var command = new UpdateDelivererCommand
        {
            Id = request.Id!,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            VehicleType = request.VehicleType,
            DelivererStatus = request.DelivererStatus
        };

        var result = await updateDelivererHandler.Handle(command, ct);

        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Code.Equals("DelivererError_NotFound", StringComparison.OrdinalIgnoreCase) || e.Code.Equals("Deliverer.NotFound", StringComparison.OrdinalIgnoreCase)))
            {
                Response.ShowError("Traženi dostavljač nije pronađen");
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                if (error.Code.Contains("Email", StringComparison.OrdinalIgnoreCase))
                    ModelState.AddModelError("Email", error.Description);
                else
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return PartialView("_DeliverersForm", request);
        }

        Response.ShowSuccess("Dostavljač je uspešno ažuriran");
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> DeleteModal(string id, CancellationToken ct = default)
    {
        var delivererResult = await getDelivererByIdHandler.Handle(id, ct);
        if (!delivererResult.IsSuccess)
        {
            Response.ShowError("Traženi dostavljač nije pronađen");
            return RedirectToAction("Index");
        }

        return PartialView("_DeleteDelivererModal", delivererResult.Payload);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        string id,
        string search = "",
        string sortBy = "",
        VehicleType? vehicleType = null,
        DelivererStatus? delivererStatus = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var command = new DeleteDelivererCommand(id);
        var result = await deleteDelivererHandler.Handle(command, ct);

        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Code.Equals("DelivererError_NotFound", StringComparison.OrdinalIgnoreCase) || e.Code.Equals("Deliverer.NotFound", StringComparison.OrdinalIgnoreCase)))
            {
                Response.ShowError("Traženi dostavljač nije pronađen");
                return RedirectToAction("Index");
            }
        }

        Response.ShowSuccess("Dostavljač je uspešno obrisan");

        if (Request.IsHtmx())
        {
            return await List(search, sortBy, vehicleType, delivererStatus, page, pageSize, ct);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(
        string id,
        DelivererStatus status,
        string search = "",
        string sortBy = "",
        VehicleType? vehicleType = null,
        DelivererStatus? delivererStatus = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var command = new UpdateDelivererStatusCommand(id, status);
        var result = await updateStatusHandler.Handle(command, ct);

        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Code.Equals("DelivererError_NotFound", StringComparison.OrdinalIgnoreCase) || e.Code.Equals("Deliverer.NotFound", StringComparison.OrdinalIgnoreCase)))
            {
                Response.ShowError("Traženi dostavljač nije pronađen");
                return RedirectToAction("Index");
            }
        }

        Response.ShowSuccess("Status dostavljača je uspešno ažuriran");

        if (Request.IsHtmx())
        {
            return await List(search, sortBy, vehicleType, delivererStatus, page, pageSize, ct);
        }

        return RedirectToAction("Index");
    }
}
