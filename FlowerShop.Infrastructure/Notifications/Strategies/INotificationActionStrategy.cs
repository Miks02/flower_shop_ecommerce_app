using FlowerShop.Domain.Entities.Notifications;

namespace FlowerShop.Infrastructure.Notifications.Strategies;

public interface INotificationActionStrategy
{
    NotificationEntityType EntityType { get; }
    string? GetActionUrl(int? entityId);
}