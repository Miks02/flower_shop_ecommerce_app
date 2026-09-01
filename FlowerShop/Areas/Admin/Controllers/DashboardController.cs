using FlowerShop.Web.Controllers;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    
    public IActionResult Index()
    {

        if (Request.IsHtmx())
            return PartialView("_DashboardPartial");
        
        return View();
    }
}