using FlowerShop.Domain.Entities.Notifications;

namespace FlowerShop.Application.Common.Abstractions;

public interface INotificationResolver
{
    string? GetActionUrl(NotificationEntityType entityType, int? entityId);
    string GetIconClass(NotificationType notificationType);
    string GetColorClass(NotificationType notificationType);
    string GetBackgroundColorClass(NotificationType notificationType);
}