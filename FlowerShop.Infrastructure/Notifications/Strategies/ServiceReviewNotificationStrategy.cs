using FlowerShop.Domain.Entities.Notifications;

namespace FlowerShop.Infrastructure.Notifications.Strategies;

public class ServiceReviewNotificationStrategy : INotificationActionStrategy
{
    public NotificationEntityType EntityType => NotificationEntityType.ServiceReview;

    public string? GetActionUrl(int? entityId)
    {
        if (!entityId.HasValue)
            return null;

        return $"/User/Profile/Reviews";
    }
}