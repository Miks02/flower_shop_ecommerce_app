using FlowerShop.Application.Features.Catalogue.Queries;
using FlowerShop.Web.ViewModels;
using Htmx;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Controllers;

public class CatalogueController(
    ILogger<CatalogueController> logger,
    GetCatalogSummaryHandler getCatalogSummaryHandler,
    GetCatalogHandler getCatalogHandler) : BaseController(logger)
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] GetCatalogSummaryQuery request, CancellationToken ct = default)
    {
        var summary = await getCatalogSummaryHandler.Handle(request, ct);

        var vm = new CatalogueViewModel
        {
            PagedProducts = summary.PagedProducts,
            Categories = summary.Categories,
            Occasions = summary.Occasions,
            CategoryIds = request.CategoryIds,
            OccasionIds = request.OccasionIds,
            PriceRange = request.PriceRange,
            Page = request.Page,
            PageSize = request.PageSize,
            Sort = request.Sort
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] GetCatalogQuery request, CancellationToken ct = default)
    {
        var pagedProducts = await getCatalogHandler.Handle(request, ct);

        if (Request.IsHtmx())
            return PartialView("Partial/_ProductList", pagedProducts);

        return RedirectToAction(nameof(Index), new
        {
            request.PriceRange,
            request.Page,
            request.PageSize,
            request.Sort,
            request.CategoryIds,
            request.OccasionIds
        });
    }
}