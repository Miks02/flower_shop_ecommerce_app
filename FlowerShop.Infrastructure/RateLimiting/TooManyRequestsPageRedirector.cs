using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FlowerShop.Infrastructure.RateLimiting;

public static class TooManyRequestsPageRedirector
{
    public const string TooManyRequestsTempDataKey = "TooManyRequests";
    private const string TooManyRequestsPagePath = "/Error/TooManyRequests";

    public static void RedirectToTooManyRequestsPage(HttpContext httpContext, ITempDataDictionaryFactory tempDataDictionaryFactory)
    {
        var tempData = tempDataDictionaryFactory.GetTempData(httpContext);
        tempData[TooManyRequestsTempDataKey] = "toomanyrequests";
        tempData.Save();

        if (httpContext.Request.Headers.ContainsKey("HX-Request"))
        {
            httpContext.Response.Headers["HX-Redirect"] = TooManyRequestsPagePath;
            httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            return;
        }

        httpContext.Response.Redirect(TooManyRequestsPagePath);
    }
}
