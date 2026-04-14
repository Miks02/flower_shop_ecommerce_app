using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Controllers;

public class ContactController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}