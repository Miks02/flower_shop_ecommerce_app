using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FlowerShop.Infrastructure.Filters;

public class NotFoundResultFilter : IResultFilter
{
    private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

    public NotFoundResultFilter(ITempDataDictionaryFactory tempDataDictionaryFactory)
    {
        _tempDataDictionaryFactory = tempDataDictionaryFactory;
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is NotFoundObjectResult notFoundObjectResult)
        {
            var tempData = _tempDataDictionaryFactory.GetTempData(context.HttpContext);
            
            tempData[NotFoundPageRedirector.NotFoundTempDataKey] = "notfound";
            
            if (notFoundObjectResult.Value != null)
            {
                if (notFoundObjectResult.Value is IDictionary<string, object> dictionary && 
                    dictionary.TryGetValue("message", out var messageObj))
                {
                    tempData[NotFoundPageRedirector.NotFoundMessageTempDataKey] = messageObj?.ToString();
                }
                else if (notFoundObjectResult.Value.GetType().GetProperty("message") != null)
                {
                    var messageProperty = notFoundObjectResult.Value.GetType().GetProperty("message");
                    var messageValue = messageProperty?.GetValue(notFoundObjectResult.Value)?.ToString();
                    if (!string.IsNullOrEmpty(messageValue))
                    {
                        tempData[NotFoundPageRedirector.NotFoundMessageTempDataKey] = messageValue;
                    }
                }
            }
            
            tempData.Save();
            
            context.Result = new RedirectToActionResult("NotFoundPage", "Error", null);
        }
        else if (context.Result is NotFoundResult)
        {
            var tempData = _tempDataDictionaryFactory.GetTempData(context.HttpContext);
            tempData[NotFoundPageRedirector.NotFoundTempDataKey] = "notfound";
            tempData.Save();
            
            context.Result = new RedirectToActionResult("NotFoundPage", "Error", null);
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}
