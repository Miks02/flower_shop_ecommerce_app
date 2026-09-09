using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace FlowerShop.Infrastructure.RateLimiting;

public static class RateLimitingRejectionHandler
{
    public static ValueTask HandleAsync(HttpContext httpContext, TimeSpan? retryAfter)
    {
        if (retryAfter.HasValue)
        {
            httpContext.Response.Headers.RetryAfter = ((int)retryAfter.Value.TotalSeconds).ToString();
        }

        var tempDataDictionaryFactory = httpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
        TooManyRequestsPageRedirector.RedirectToTooManyRequestsPage(httpContext, tempDataDictionaryFactory);

        return ValueTask.CompletedTask;
    }
}
