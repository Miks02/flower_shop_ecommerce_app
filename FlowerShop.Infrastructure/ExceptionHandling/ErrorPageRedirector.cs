using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FlowerShop.Infrastructure.ExceptionHandling;

public static class ErrorPageRedirector
{
    public const string UnhandledErrorTempDataKey = "UnhandledError";
    private const string ErrorPagePath = "/Error";

    public static void RedirectToErrorPage(HttpContext httpContext, ITempDataDictionaryFactory tempDataDictionaryFactory)
    {
        var tempData = tempDataDictionaryFactory.GetTempData(httpContext);
        tempData[UnhandledErrorTempDataKey] = "error";
        tempData.Save();

        httpContext.Response.Redirect(ErrorPagePath);
    }
}
