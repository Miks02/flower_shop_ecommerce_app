using FlowerShop.Application.Features.Catalogue.Queries.GetCatalog;
using FlowerShop.Application.Features.Catalogue.Queries.GetCatalogSummary;
using FlowerShop.Application.Features.Catalogue.Queries.GetProductDetails;
using FlowerShop.Infrastructure.Htmx;
using FlowerShop.Web.ViewModels;
using Htmx;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Controllers;

public class CatalogueController(
    ILogger<CatalogueController> logger,
    GetCatalogSummaryHandler getCatalogSummaryHandler,
    GetCatalogHandler getCatalogHandler,
    GetProductDetailsHandler getProductDetailsHandler) : BaseController(logger)
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

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct = default)
    {
        var result = await getProductDetailsHandler.Handle(new GetProductDetailsQuery { Id = id }, ct);
        if (!result.IsSuccess || result.Payload is null)
        {
            Response.ShowError("Traženi proizvod nije pronađen.");
            return RedirectToAction(nameof(Index));
        }

        var product = result.Payload;
        var vm = new ProductDetailsViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            Price = product.Price,
            PromoPrice = product.PromoPrice,
            IsOnPromotion = product.IsOnPromotion,
            DiscountType = product.DiscountType,
            Stock = product.Stock,
            CategoryName = product.CategoryName,
            Occasions = product.Occasions,
            Composition = product.Composition
        };

        return View(vm);
    }
}