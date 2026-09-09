using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace FlowerShop.Infrastructure.ExceptionHandling;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    ITempDataDictionaryFactory tempDataDictionaryFactory) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception,
            "Unhandled exception occurred while processing {Method} {Path}",
            httpContext.Request.Method, httpContext.Request.Path);

        ErrorPageRedirector.RedirectToErrorPage(httpContext, tempDataDictionaryFactory);

        return ValueTask.FromResult(true);
    }
}
