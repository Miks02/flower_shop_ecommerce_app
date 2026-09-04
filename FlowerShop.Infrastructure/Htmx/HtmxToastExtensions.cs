using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace FlowerShop.Infrastructure.Htmx;

public static class HtmxToastExtensions
{
    private const string HxTriggerHeader = "HX-Trigger";
    private const string ToastEventName = "showToast";
    public const string ToastTempDataKey = "_HtmxToastMessage";
    public const string ToastHttpContextItemKey = "_HtmxToastItem";

    public static void ShowToast(this HttpResponse response, ToastLevel level, string message, string? title = null, int duration = 4000)
    {
        var toast = new ToastMessage
        {
            Message = message,
            Title = title,
            Level = level.ToString().ToLowerInvariant(),
            Duration = duration
        };

        var httpContext = response.HttpContext;
        httpContext.Items[ToastHttpContextItemKey] = toast;

        if (httpContext.Request.Headers.ContainsKey("HX-Request"))
        {
            AppendHxTrigger(response, toast);
        }
    }

    public static void ShowSuccess(this HttpResponse response, string message, string? title = null, int duration = 4000)
    {
        response.ShowToast(ToastLevel.Success, message, title, duration);
    }

    public static void ShowError(this HttpResponse response, string message, string? title = null, int duration = 4000)
    {
        response.ShowToast(ToastLevel.Error, message, title, duration);
    }

    public static void ShowWarning(this HttpResponse response, string message, string? title = null, int duration = 4000)
    {
        response.ShowToast(ToastLevel.Warning, message, title, duration);
    }

    public static void ShowInfo(this HttpResponse response, string message, string? title = null, int duration = 4000)
    {
        response.ShowToast(ToastLevel.Info, message, title, duration);
    }

    private static void AppendHxTrigger(HttpResponse response, ToastMessage toast)
    {
        var currentHeader = response.Headers[HxTriggerHeader].ToString();
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        if (string.IsNullOrWhiteSpace(currentHeader))
        {
            var triggerPayload = new Dictionary<string, object>
            {
                { ToastEventName, toast }
            };
            response.Headers[HxTriggerHeader] = JsonSerializer.Serialize(triggerPayload, serializerOptions);
            return;
        }

        try
        {
            var existingTriggers = JsonSerializer.Deserialize<Dictionary<string, object>>(currentHeader) ?? new Dictionary<string, object>();
            existingTriggers[ToastEventName] = toast;
            response.Headers[HxTriggerHeader] = JsonSerializer.Serialize(existingTriggers, serializerOptions);
        }
        catch
        {
            var fallback = new Dictionary<string, object>
            {
                { currentHeader, string.Empty },
                { ToastEventName, toast }
            };
            response.Headers[HxTriggerHeader] = JsonSerializer.Serialize(fallback, serializerOptions);
        }
    }
}
