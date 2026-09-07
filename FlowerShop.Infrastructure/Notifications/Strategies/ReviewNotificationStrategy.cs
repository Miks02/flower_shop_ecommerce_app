using FlowerShop.Domain.Entities.Notifications;

namespace FlowerShop.Infrastructure.Notifications.Strategies;

public class ReviewNotificationStrategy : INotificationActionStrategy
{
    public NotificationEntityType EntityType => NotificationEntityType.Review;

    public string? GetActionUrl(int? entityId)
    {
        if (!entityId.HasValue)
            return null;

        return $"/User/Profile/Reviews";
    }
}