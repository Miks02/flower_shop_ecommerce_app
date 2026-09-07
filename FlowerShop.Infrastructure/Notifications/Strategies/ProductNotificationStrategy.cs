using FlowerShop.Domain.Entities.Notifications;

namespace FlowerShop.Infrastructure.Notifications.Strategies;

public class ProductNotificationStrategy : INotificationActionStrategy
{
    public NotificationEntityType EntityType => NotificationEntityType.Product;

    public string? GetActionUrl(int? entityId)
    {
        if (!entityId.HasValue)
            return null;

        return $"/Product/Details/{entityId.Value}";
    }
}