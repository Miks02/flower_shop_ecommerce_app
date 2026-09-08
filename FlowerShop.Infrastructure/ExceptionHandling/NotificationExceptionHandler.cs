using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Notifications = FlowerShop.Infrastructure.Notifications;

namespace FlowerShop.Infrastructure.ExceptionHandling;

public sealed class NotificationExceptionHandler(
    ILogger<NotificationExceptionHandler> logger,
    ITempDataDictionaryFactory tempDataDictionaryFactory) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not Notifications.NotificationException notificationException)
            return ValueTask.FromResult(false);

        logger.LogError(notificationException,
            "Notification delivery failure while processing {Method} {Path}",
            httpContext.Request.Method, httpContext.Request.Path);

        ErrorPageRedirector.RedirectToErrorPage(httpContext, tempDataDictionaryFactory);

        return ValueTask.FromResult(true);
    }
}
