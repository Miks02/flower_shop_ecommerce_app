using FlowerShop.Infrastructure.ExceptionHandling;
using FlowerShop.Infrastructure.Filters;
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

    [Route("/Error/NotFound")]
    public IActionResult NotFoundPage()
    {
        if (string.IsNullOrWhiteSpace(TempData[NotFoundPageRedirector.NotFoundTempDataKey] as string))
            return RedirectToAction(nameof(HomeController.Index), "Home");

        var message = TempData[NotFoundPageRedirector.NotFoundMessageTempDataKey] as string;
        ViewData["NotFoundMessage"] = message;

        return View("NotFound");
    }
}
