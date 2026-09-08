using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlowerShop.Infrastructure.ExceptionHandling;

public sealed class DbUpdateExceptionHandler(
    ILogger<DbUpdateExceptionHandler> logger,
    ITempDataDictionaryFactory tempDataDictionaryFactory) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DbUpdateException dbUpdateException)
            return ValueTask.FromResult(false);

        var affectedEntities = string.Join(", ", dbUpdateException.Entries.Select(e => e.Entity.GetType().Name).Distinct());

        logger.LogCritical(dbUpdateException,
            "Database update failure while processing {Method} {Path}. Affected entities: {AffectedEntities}",
            httpContext.Request.Method, httpContext.Request.Path, affectedEntities);

        ErrorPageRedirector.RedirectToErrorPage(httpContext, tempDataDictionaryFactory);

        return ValueTask.FromResult(true);
    }
}
