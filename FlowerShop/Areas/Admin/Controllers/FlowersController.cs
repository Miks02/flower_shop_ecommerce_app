using System.Text.Json;
using FlowerShop.Application.Features.Flowers.Commands.AddFlower;
using FlowerShop.Application.Features.Flowers.Commands.DeleteFlower;
using FlowerShop.Application.Features.Flowers.Commands.UpdateFlowerStock;
using FlowerShop.Application.Features.Flowers.Queries;
using FlowerShop.Infrastructure.Htmx;
using FlowerShop.Web.Areas.Admin.Models.Flowers;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class FlowersController(
    GetFlowersHandler getFlowersHandler,
    GetFlowerByIdHandler getFlowerByIdHandler,
    AddFlowerHandler addFlowerHandler,
    UpdateFlowerStockHandler updateFlowerStockHandler,
    DeleteFlowerHandler deleteFlowerHandler) : Controller
{
    [HttpGet]
    public IActionResult AddModal(string? selectedFlowersJson)
    {
        var vm = new AddFlowerViewModel
        {
            SelectedFlowersJson = selectedFlowersJson ?? "[]"
        };

        return PartialView("_AddFlowerModal", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddFlowerViewModel request, CancellationToken ct = default)
    {
        var command = new AddFlowerCommand
        {
            Name = request.Name,
            Color = request.Color,
            FlowerCategory = request.FlowerCategory,
            Stock = request.Stock
        };

        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                ModelState.AddModelError(string.Empty, error.ErrorMessage);

            return PartialView("_AddFlowerModal", request);
        }

        //var validationResult = await addFlowerValidator.ValidateAsync(command, ct);
        //if (!validationResult.IsValid)
        //{
        //    foreach (var error in validationResult.Errors)
        //        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

        //    return PartialView("_AddFlowerModal", request);
        //}

        var result = await addFlowerHandler.Handle(command, ct);

        if (!result.IsSuccess)
        {
            foreach (var error in result.Errors)
            {
                if (error.Code.Equals("FlowerError_AlreadyExists"))
                    ModelState.AddModelError(nameof(request.Name), "Cvet sa navedenim imenom, bojom i kategorijom već postoji.");
                else
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return PartialView("_AddFlowerModal", request);
        }

        Response.ShowSuccess("Cvet je uspešno dodat");
        var listVm = await BuildFlowerSelectionListViewModel(request.SelectedFlowersJson, ct);
        return PartialView("_FlowerSelectionListOob", listVm);
    }

    [HttpGet]
    public async Task<IActionResult> UpdateStockModal(int id, string? selectedFlowersJson, CancellationToken ct = default)
    {
        var flowerResult = await getFlowerByIdHandler.Handle(id, ct);
        if (!flowerResult.IsSuccess)
        {
            Response.ShowError("Traženi cvet nije pronađen");
            return RedirectToAction("Index", "Products");
        }

        var vm = new UpdateFlowerStockViewModel
        {
            Id = flowerResult.Payload!.Id,
            Name = flowerResult.Payload.Name,
            CurrentStock = flowerResult.Payload.Stock,
            SelectedFlowersJson = selectedFlowersJson ?? "[]"
        };

        return PartialView("_UpdateFlowerStockModal", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStock(UpdateFlowerStockViewModel request, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                ModelState.AddModelError(string.Empty, error.ErrorMessage);

            return PartialView("_UpdateFlowerStockModal", request);
        }

        var command = new UpdateFlowerStockCommand
        {
            Id = request.Id,
            Quantity = request.Quantity
        };

        var result = await updateFlowerStockHandler.Handle(command, ct);

        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Code.Equals("FlowerError_NotFound")))
            {
                Response.ShowError("Traženi cvet nije pronađen");
                return RedirectToAction("Index", "Products");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return PartialView("_UpdateFlowerStockModal", request);
        }

        Response.ShowSuccess("Stanje cveta je uspešno ažurirano");
        var listVm = await BuildFlowerSelectionListViewModel(request.SelectedFlowersJson, ct);
        return PartialView("_FlowerSelectionListOob", listVm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, string? selectedFlowersJson, CancellationToken ct = default)
    {
        var command = new DeleteFlowerCommand(id);
        var result = await deleteFlowerHandler.Handle(command, ct);

        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Code.Equals("FlowerError_NotFound")))
            {
                Response.ShowError("Traženi cvet nije pronađen");
                return RedirectToAction("Index", "Products");
            }

            var flowerResult = await getFlowerByIdHandler.Handle(id, ct);

            var vm = new UpdateFlowerStockViewModel
            {
                Id = id,
                Name = flowerResult.IsSuccess ? flowerResult.Payload!.Name : string.Empty,
                CurrentStock = flowerResult.IsSuccess ? flowerResult.Payload!.Stock : 0,
                SelectedFlowersJson = selectedFlowersJson ?? "[]"
            };

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return PartialView("_UpdateFlowerStockModal", vm);
        }

        Response.ShowSuccess("Cvet je uspešno obrisan");
        var deletedListVm = await BuildFlowerSelectionListViewModel(selectedFlowersJson, ct);
        return PartialView("_FlowerSelectionListOob", deletedListVm);
    }

    private async Task<FlowerSelectionListViewModel> BuildFlowerSelectionListViewModel(string? selectedFlowersJson, CancellationToken ct)
    {
        var flowers = await getFlowersHandler.Handle(ct);
        var selectedFlowers = new List<SelectedFlowerDto>();

        if (!string.IsNullOrWhiteSpace(selectedFlowersJson))
        {
            try
            {
                selectedFlowers = JsonSerializer.Deserialize<List<SelectedFlowerDto>>(selectedFlowersJson) ?? [];
            }
            catch (JsonException)
            {
                selectedFlowers = [];
            }
        }

        return new FlowerSelectionListViewModel
        {
            AvailableFlowers = flowers.Flowers,
            SelectedFlowers = selectedFlowers
        };
    }
}
