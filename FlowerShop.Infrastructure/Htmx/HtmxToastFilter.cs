using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FlowerShop.Infrastructure.Htmx;

public class HtmxToastFilter : IActionFilter, IResultFilter
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller is Controller controller)
        {
            if (controller.TempData.TryGetValue(HtmxToastExtensions.ToastTempDataKey, out var toastObj) && toastObj is string toastJson)
            {
                try
                {
                    var toast = JsonSerializer.Deserialize<ToastMessage>(toastJson, _jsonSerializerOptions);
                    if (toast != null)
                    {
                        controller.ViewData[HtmxToastExtensions.ToastTempDataKey] = toast;
                        if (context.HttpContext.Request.Headers.ContainsKey("HX-Request"))
                        {
                            context.HttpContext.Response.ShowToast(
                                Enum.TryParse<ToastLevel>(toast.Level, true, out var lvl) ? lvl : ToastLevel.Info,
                                toast.Message,
                                toast.Title,
                                toast.Duration);
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        var httpContext = context.HttpContext;
        if (httpContext.Items.TryGetValue(HtmxToastExtensions.ToastHttpContextItemKey, out var toastObj) && toastObj is ToastMessage toast)
        {
            var isRedirect = context.Result is RedirectResult ||
                             context.Result is RedirectToActionResult ||
                             context.Result is RedirectToRouteResult ||
                             context.Result is LocalRedirectResult ||
                             (httpContext.Response.StatusCode >= 300 && httpContext.Response.StatusCode < 400);

            if (isRedirect)
            {
                if (context.Controller is Controller controller)
                {
                    controller.TempData[HtmxToastExtensions.ToastTempDataKey] = JsonSerializer.Serialize(toast, _jsonSerializerOptions);
                }
                
            }
            else if (context.Controller is Controller controller)
            {
                controller.ViewData[HtmxToastExtensions.ToastTempDataKey] = toast;
            }
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }

   
}
