using FlowerShop.Infrastructure.ExceptionHandling;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Controllers;

public class ErrorController : Controller
{
    [Route("/Error")]
    public IActionResult Index()
    {
        if (string.IsNullOrWhiteSpace(TempData[ErrorPageRedirector.UnhandledErrorTempDataKey] as string))
            return RedirectToAction(nameof(HomeController.Index), "Home");

        return View();
    }
}
