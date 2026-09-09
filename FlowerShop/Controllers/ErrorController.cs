using FlowerShop.Infrastructure.ExceptionHandling;
using FlowerShop.Infrastructure.Filters;
using FlowerShop.Infrastructure.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FlowerShop.Web.Controllers;

[DisableRateLimiting]
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

    [Route("/Error/TooManyRequests")]
    public IActionResult TooManyRequests()
    {
        if (string.IsNullOrWhiteSpace(TempData[TooManyRequestsPageRedirector.TooManyRequestsTempDataKey] as string))
            return RedirectToAction(nameof(HomeController.Index), "Home");

        return View("TooManyRequests");
    }
}
