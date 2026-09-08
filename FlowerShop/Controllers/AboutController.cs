using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Controllers;

public class AboutController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
