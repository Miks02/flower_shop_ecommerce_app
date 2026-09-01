using FlowerShop.Application.Features.Flowers.Queries;
using FlowerShop.Application.Features.Products.Commands.AddProduct;
using FlowerShop.Application.Features.Products.Commands.DeleteProduct;
using FlowerShop.Application.Features.Products.Commands.UpdateProduct;
using FlowerShop.Application.Features.Products.Queries.GetProductReferenceData;
using FlowerShop.Application.Features.Products.Queries.GetProducts;
using FlowerShop.Application.Features.Products.Queries.GetProductsSummary;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.SharedKernel.Results;
using FluentValidation;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FlowerShop.Application.Features.Products.Queries.GetProductById;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Web.Areas.Admin.Models.Products;
using AddFlowerItemDto = FlowerShop.Application.Features.Products.Commands.AddProduct.FlowerItemDto;
using UpdateFlowerItemDto = FlowerShop.Application.Features.Products.Commands.UpdateProduct.FlowerItemDto;

namespace FlowerShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController(
    GetProductsHandler getProductsHandler,
    GetProductByIdHandler getProductByIdHandler,
    GetProductsSummaryHandler getSummaryHandler,
    GetProductReferenceDataHandler getRefDataHandler,
    GetFlowersHandler getFlowersHandler,
    AddProductHandler addProductHandler,
    UpdateProductHandler updateProductHandler,
    DeleteProductHandler deleteProductHandler,
    IProductRepository productRepo
    ) : Controller
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
            SelectedFlowers = new List<AddFlowerItemDto>(),
            AvailableFlowers = flowers.Flowers,
            AvailableCategories = productRefData.Categories,
            AvailableOccasions = productRefData.Occasions
        };
        
        if(Request.IsHtmx())
            return PartialView("_ProductsForm", vm);
        
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
    {
        var productResult = await getProductByIdHandler.Handle(id, ct);
        if(!productResult.IsSuccess)
            return NotFound();

        var product = productResult.Payload;

        var productRefData = await getRefDataHandler.Handle(ct);
        var flowers = await getFlowersHandler.Handle(ct);

        var vm = new ProductFormViewModel
        {
            Id = product!.Id,
            Name = product.Name,
            ProductImageUrl = product.ImageUrl,
            Description = product.Description ?? string.Empty,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            OccasionIds = product.OccasionIds.ToList(),
            SelectedFlowers = product.Flowers.ToList(),
            CreatedByName = product.CreatedBy,
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
            if (request.ProductImage != null && request.ProductImage.Length > 0)
                ModelState.AddModelError("ProductImage", "Molimo vas ponovo unesite sliku proizvoda");

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
            if (request.ProductImage != null && request.ProductImage.Length > 0)
                ModelState.AddModelError("ProductImage", "Molimo vas ponovo unesite sliku proizvoda");
            foreach (var error in result.Errors)
            {
                if (error.Code.Equals("FlowerError_InsufficientStock")) 
                    ModelState.AddModelError("SelectedFlowers", "Neki od izabranih cvetova nisu na stanju");
                else if (error.Code.Equals("ProductError_ProductAlreadyExists"))
                    ModelState.AddModelError("Name", "Proizvod sa navedenim imenom već postoji");
                else
                    ModelState.AddModelError(string.Empty, error.Description);
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
        var command = new UpdateProductCommand
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId,
            Occasions = request.OccasionIds,
            Flowers = request.SelectedFlowers.Select(f => new UpdateFlowerItemDto(f.Id, f.Quantity)).ToList(),
            ProductImage = request.ProductImage
        };

        if (!ModelState.IsValid)
        {
            if (request.ProductImage != null && request.ProductImage.Length > 0)
                ModelState.AddModelError("ProductImage", "Molimo vas ponovo unesite sliku proizvoda");

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

        var result = await updateProductHandler.Handle(command, ct);

        if (!result.IsSuccess)
        {
            if (request.ProductImage != null && request.ProductImage.Length > 0)
                ModelState.AddModelError("ProductImage", "Molimo vas ponovo unesite sliku proizvoda");
            foreach (var error in result.Errors)
            {
                if (error.Code.Equals("FlowerError_InsufficientStock"))
                    ModelState.AddModelError("SelectedFlowers", "Neki od izabranih cvetova nisu na stanju");
                else if (error.Code.Equals("ProductError_ProductNotFound")) 
                    return RedirectToAction("Index", "Home");
                else if (error.Code.Equals("ProductError_ProductAlreadyExists"))
                    ModelState.AddModelError("Name", "Proizvod sa navedenim imenom već postoji");
                else
                    ModelState.AddModelError(string.Empty, error.Description);
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

    [HttpGet]
    public async Task<IActionResult> DeleteModal(int id, CancellationToken ct = default)
    {
        var productResult = await getProductByIdHandler.Handle(id, ct);
        if (!productResult.IsSuccess)
            return NotFound();

        return PartialView("_DeleteProductModal", productResult.Payload);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id,
        bool isDeleted,
        IReadOnlyList<int> occasionIds,
        string searchBy = "",
        string sortBy = "name",
        int? categoryId = null,
        CancellationToken ct = default)
    {
        var command = new DeleteProductCommand(id);
        var result = await deleteProductHandler.Handle(command, ct);

        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Code.Equals("ProductError_ProductNotFound")))
                return NotFound();
        }

        if (Request.IsHtmx())
        {
            return await List(isDeleted, occasionIds, searchBy, sortBy, categoryId, ct);
        }

        return RedirectToAction("Index");
    }
}