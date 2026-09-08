using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Catalogue.Queries.GetCatalog;
using FlowerShop.Application.Features.Catalogue.Queries.GetCatalogSummary;
using FlowerShop.Application.Features.Catalogue.Queries.GetProductDetails;
using FlowerShop.Application.Features.ProductReviews.Commands.CreateProductReview;
using FlowerShop.Application.Features.ProductReviews.Commands.DeleteProductReview;
using FlowerShop.Infrastructure.Htmx;
using FlowerShop.Web.ViewModels;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Controllers;

public class CatalogueController(
    ILogger<CatalogueController> logger,
    IUserProvider userProvider,
    GetCatalogSummaryHandler getCatalogSummaryHandler,
    GetCatalogHandler getCatalogHandler,
    GetProductDetailsHandler getProductDetailsHandler,
    CreateProductReviewHandler createProductReviewHandler,
    DeleteProductReviewHandler deleteProductReviewHandler) : BaseController(logger)
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
            Flowers = summary.Flowers,
            CategoryIds = request.CategoryIds,
            OccasionIds = request.OccasionIds,
            FlowerIds = request.FlowerIds,
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
            request.OccasionIds,
            request.FlowerIds
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct = default)
    {
        var currentUserId = User.Identity?.IsAuthenticated == true && userProvider.IsUser()
            ? userProvider.GetCurrentUserId()
            : null;

        var result = await getProductDetailsHandler.Handle(new GetProductDetailsQuery { Id = id, CurrentUserId = currentUserId }, ct);
        if (!result.IsSuccess || result.Payload is null)
        {
            Response.ShowError("Traženi proizvod nije pronađen.");
            return RedirectToAction(nameof(Index));
        }

        return View(BuildProductDetailsViewModel(result.Payload));
    }

    [HttpPost]
    [Authorize(Roles = "User")]
    [ValidateAntiForgeryToken]
    [Route("/Catalogue/Reviews/{id:int}")]
    public async Task<IActionResult> SubmitProductReview(int id, [FromForm] SubmitProductReviewRequest request, CancellationToken ct = default)
    {
        var userId = userProvider.GetCurrentUserId();

        if (ModelState.IsValid)
        {
            var command = new CreateProductReviewCommand(userId, id, request.Rating, request.Comment);
            var result = await createProductReviewHandler.Handle(command, ct);

            if (!result.IsSuccess)
                Response.ShowError(result.Errors[0].Description);
            else
                Response.ShowSuccess("Hvala vam na ostavljenoj recenziji proizvoda!");
        }
        else
        {
            Response.ShowWarning("Proverite sva polja pre slanja recenzije.");
        }

        var refreshed = await getProductDetailsHandler.Handle(new GetProductDetailsQuery { Id = id, CurrentUserId = userId }, ct);
        if (!refreshed.IsSuccess || refreshed.Payload is null)
        {
            Response.ShowError("Traženi proizvod nije pronađen.");
            return NotFound();
        }

        return PartialView("Partial/_ProductReviews", BuildProductDetailsViewModel(refreshed.Payload));
    }

    [HttpPost]
    [Authorize(Roles = "User")]
    [Route("/Catalogue/Reviews/{id:int}/Delete/{reviewId:int}")]
    public async Task<IActionResult> DeleteProductReview(int id, int reviewId, CancellationToken ct = default)
    {
        var userId = userProvider.GetCurrentUserId();

        var result = await deleteProductReviewHandler.Handle(new DeleteProductReviewCommand(reviewId, userId), ct);

        if (!result.IsSuccess)
            Response.ShowError(result.Errors[0].Description);
        else
            Response.ShowSuccess("Recenzija je obrisana.");

        var refreshed = await getProductDetailsHandler.Handle(new GetProductDetailsQuery { Id = id, CurrentUserId = userId }, ct);
        if (!refreshed.IsSuccess || refreshed.Payload is null)
        {
            Response.ShowError("Traženi proizvod nije pronađen.");
            return NoContent();
        }

        return PartialView("Partial/_ProductReviews", BuildProductDetailsViewModel(refreshed.Payload));
    }

    private static ProductDetailsViewModel BuildProductDetailsViewModel(GetProductDetailsResponse product)
    {
        return new ProductDetailsViewModel
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
            Composition = product.Composition,
            AverageRating = product.AverageRating,
            ReviewCount = product.ReviewCount,
            CurrentUserReviewId = product.CurrentUserReviewId,
            Reviews = product.Reviews
        };
    }
}