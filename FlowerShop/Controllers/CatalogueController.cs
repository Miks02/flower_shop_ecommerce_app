using System.Collections;
using FlowerShop.Models;
using FlowerShop.Services.Interfaces;
using FlowerShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Controllers;

public class CatalogueController : BaseController
{

    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IOccasionService _occasionService;
    
    public CatalogueController
    (
        ILogger<CatalogueController> logger, 
        IProductService productService,
        ICategoryService categoryService,
        IOccasionService occasionService       
        ) : base(logger)
    {
        _productService = productService;       
        _categoryService = categoryService;
        _occasionService = occasionService;       
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var products =  (await _productService.GetAll()).ToList();
        var categories = (await _categoryService.GetAll()).ToList();
        var occasions = (await _occasionService.GetAll()).ToList();

        var vm = new CatalogueViewModel
        {
            Products = products,
            Categories = categories,
            Occasions = occasions
        };
        return View(vm);
    }
}