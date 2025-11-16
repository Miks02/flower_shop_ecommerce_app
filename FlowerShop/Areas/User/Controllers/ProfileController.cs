using FlowerShop.Controllers;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = "User")]
public class ProfileController : BaseController
{

    public ProfileController(ILogger<BaseController> logger) : base(logger)
    {
        
    }
   
    [HttpGet("/User/Profile/")]
    public IActionResult Index()
    {

        if (Request.IsHtmx())
            return ViewComponent("Profile");
        
        return View();
    }

    [HttpGet]
    public IActionResult Settings()
    {
        if (Request.IsHtmx())
        {
            return ViewComponent("Settings");
        }

        return View();
    }
}