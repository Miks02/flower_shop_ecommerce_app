using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Controllers;

public class ContactController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}