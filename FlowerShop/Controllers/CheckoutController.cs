using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Controllers;

[Authorize(Roles = "User")]
public class CheckoutController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}