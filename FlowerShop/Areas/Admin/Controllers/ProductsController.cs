using FlowerShop.Application.Features.Flowers.Queries;
using FlowerShop.Application.Features.Products.Commands.AddProduct;
using FlowerShop.Application.Features.Products.Queries.GetProductReferenceData;
using FlowerShop.Application.Features.Products.Queries.GetProducts;
using FlowerShop.Application.Features.Products.Queries.GetProductsSummary;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;
using FlowerShop.Web.Areas.Admin.Views.Products;
using FluentValidation;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FlowerShop.Domain.Entities.Flowers;

namespace FlowerShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController(
    GetProductsHandler getProductsHandler,
    GetProductsSummaryHandler getSummaryHandler,
    GetProductReferenceDataHandler getRefDataHandler,
    GetFlowersHandler getFlowersHandler,
    AddProductHandler addProductHandler,
    IProductRepository productRepo) : Controller
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
    
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        var productRefData = await getRefDataHandler.Handle(ct);
        var flowers = await getFlowersHandler.Handle(ct);

        var vm = new ProductFormViewModel
        {
            SelectedFlowers = new List<FlowerItemDto>(),
            AvailableFlowers = flowers.Flowers,
            AvailableCategories = productRefData.Categories,
            AvailableOccasions = productRefData.Occasions
        };
        
        if(Request.IsHtmx())
            return PartialView("_ProductsForm", vm);
        
        return View("_ProductsForm", vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
    {
        var product = await productRepo.GetByIdAsync(id, ct);
        if (product == null) return NotFound();

        var productRefData = await getRefDataHandler.Handle(ct);
        var flowers = await getFlowersHandler.Handle(ct);

        var vm = new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description ?? string.Empty,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            OccasionIds = product.Occasions.Select(o => o.Id).ToList(),
            SelectedFlowers = product.ProductFlowers.Select(pf => new FlowerItemDto(pf.FlowerId, pf.Quantity)).ToList(),
            CreatedByName = product.User?.UserName ?? "N/A",
            CreatedAt = product.CreatedAt,
            IsDeleted = product.IsDeleted,
            AvailableFlowers = flowers.Flowers,
            AvailableCategories = productRefData.Categories,
            AvailableOccasions = productRefData.Occasions
        };

        if (Request.IsHtmx())
            return PartialView("_ProductsForm", vm);

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductFormViewModel request, CancellationToken ct = default)
    {
        var command = new AddProductCommand
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId,
            Occasions = request.OccasionIds,
            Flowers = request.SelectedFlowers,
            ProductImage = request.ProductImage!
        };
        
        if (!ModelState.IsValid)
        {
            if (request.ProductImage.Length > 0)
            {
                ModelState.AddModelError("ProductImage", "Molimo vas ponovo unesite sliku proizvoda");
                request.ProductImage = null;
            }

            var productRefData = await getRefDataHandler.Handle(ct);
            var flowers = await getFlowersHandler.Handle(ct);
            
            var vm = request with
            {
                AvailableFlowers = flowers.Flowers,
                AvailableCategories = productRefData.Categories,
                AvailableOccasions = productRefData.Occasions
            };
            
            return PartialView("_ProductsForm", vm);
        }
        var result = await addProductHandler.Handle(command, ct);
        
        if (!result.IsSuccess)
        {
            foreach (var error in result.Errors)
            {
                var key = error.Code switch
                {
                    var c when c.Contains("Category") => "CategoryId",
                    var c when c.Contains("Occasion") => "OccasionIds",
                    var c when c.Contains("Flower") => "SelectedFlowers",
                    _ => ""
                };
                ModelState.AddModelError(key, error.Description);
            }
            var productRefData = await getRefDataHandler.Handle(ct);
            var flowers = await getFlowersHandler.Handle(ct);
            var vm = request with
            {
                AvailableFlowers = flowers.Flowers,
                AvailableCategories = productRefData.Categories,
                AvailableOccasions = productRefData.Occasions
            };
            return PartialView("_ProductsForm", vm);
        }

        return RedirectToAction("Index");

    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProductFormViewModel request, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            if (request.ProductImage.Length > 0)
            {
                ModelState.AddModelError("ProductImage", "Molimo vas ponovo unesite sliku proizvoda");
                request.ProductImage = null;
            }

            var productRefData = await getRefDataHandler.Handle(ct);
            var flowers = await getFlowersHandler.Handle(ct);
            var vm = request with
            {
                AvailableFlowers = flowers.Flowers,
                AvailableCategories = productRefData.Categories,
                AvailableOccasions = productRefData.Occasions
            };
            return PartialView("_ProductsForm", vm);
        }

        return await List(false, [], ct: ct);
    }
    
}