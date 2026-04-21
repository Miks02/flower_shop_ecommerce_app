using FlowerShop.Application.Features.Products.Queries.GetProducts;
using FlowerShop.Application.Features.Products.Queries.GetProductsSummary;
using FlowerShop.SharedKernel.Results;
using FlowerShop.Web.Areas.Admin.Views.Products;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController(GetProductsHandler getProductsHandler, GetProductsSummaryHandler getSummaryHandler) : Controller
{
    
    [HttpGet]
    public async Task<IActionResult> Index(
        bool isDeleted,
        IReadOnlyList<int> occasionIds,
        string searchBy = "", 
        string sortBy = "name", 
        int? categoryId = null, 
        CancellationToken ct = default)
    {
        var query = new GetProductsSummaryQuery
        {
            SearchBy = searchBy,
            SortBy = sortBy,
            CategoryId = categoryId,
            IsDeleted = isDeleted,
            OccasionIds = occasionIds
        };

        var summary = await getSummaryHandler.Handle(query, ct);
        
        var pagedProductsListVm = summary.PagedProducts.Items
            .Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                ProductImage = p.ProductImage,
                Price = p.Price,
                Stock = p.Stock,
                CategoryName = p.CategoryName,
                CreatedAt = p.CreatedAt,
                IsDeleted = p.IsDeleted,
                Occasions = p.Occasions,
                FlowerNames = p.ProductFlowers.Select(pf => pf.FlowerName).ToList()
            }).ToList();
            
        
        var vm = new PagedResult<ProductListViewModel>(pagedProductsListVm,  query.PageIndex, query.PageSize, pagedProductsListVm.Count, summary.PagedProducts.TotalCount);

        var summaryVm = new ProductSummaryViewModel
        {
            Categories = summary.Categories,
            Occasions = summary.Occasions,
            PagedProducts = vm
        };
        
        if(Request.IsHtmx())
            return PartialView("_ProductsPage", summaryVm);
        
        return View(summaryVm);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        bool isDeleted,
        IReadOnlyList<int> occasionIds,
        string searchBy = "",
        string sortBy = "name",
        int? categoryId = null,
        CancellationToken ct = default)
    {
        var query = new GetProductsQuery
        {
            SearchBy = searchBy,
            SortBy = sortBy,
            CategoryId = categoryId,
            IsDeleted = isDeleted,
            OccasionIds = occasionIds
        };

        var pagedProducts = await getProductsHandler.Handle(query, ct);
        
        var pagedProductsListVm = pagedProducts.Items
            .Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                ProductImage = p.ProductImage,
                Price = p.Price,
                Stock = p.Stock,
                CategoryName = p.CategoryName,
                CreatedAt = p.CreatedAt,
                IsDeleted = p.IsDeleted,
                Occasions = p.Occasions,
                FlowerNames = p.ProductFlowers.Select(pf => pf.FlowerName).ToList()
            }).ToList();
            
        
        var vm = new PagedResult<ProductListViewModel>(pagedProductsListVm,  query.PageIndex, query.PageSize, pagedProductsListVm.Count, pagedProducts.TotalCount);

        if(Request.IsHtmx())
            return PartialView("_ProductsList", vm);
        
        return RedirectToAction("Index", vm);
    }
}