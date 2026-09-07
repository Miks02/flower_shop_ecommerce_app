using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Notifications;
using FlowerShop.Infrastructure.Notifications.Strategies;

namespace FlowerShop.Infrastructure.Notifications;

public class NotificationResolver(IEnumerable<INotificationActionStrategy> strategies) : INotificationResolver
{
    public string? GetActionUrl(NotificationEntityType entityType, int? entityId)
    {
        var strategy = strategies.FirstOrDefault(s => s.EntityType == entityType);
        return strategy?.GetActionUrl(entityId);
    }

    public string GetIconClass(NotificationType notificationType)
    {
        return notificationType switch
        {
            NotificationType.Information => "fa-solid fa-circle-info",
            NotificationType.Success => "fa-solid fa-circle-check",
            NotificationType.Warning => "fa-solid fa-triangle-exclamation",
            NotificationType.Error => "fa-solid fa-circle-xmark",
            _ => "fa-solid fa-bell"
        };
    }

    public string GetColorClass(NotificationType notificationType)
    {
        return notificationType switch
        {
            NotificationType.Information => "text-blue-400",
            NotificationType.Success => "text-emerald-400",
            NotificationType.Warning => "text-amber-400",
            NotificationType.Error => "text-red-400",
            _ => "text-gray-400"
        };
    }
    
    public string GetBackgroundColorClass(NotificationType notificationType)
    {
        return notificationType switch
        {
            NotificationType.Information => "bg-blue-900/90",
            NotificationType.Success => "bg-emerald-900/90",
            NotificationType.Warning => "bg-amber-900/90",
            NotificationType.Error => "bg-red-900/90",
            _ => "bg-gray-900/90"
        };
    }
}