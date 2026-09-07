using FlowerShop.Domain.Entities.Notifications;

namespace FlowerShop.Infrastructure.Notifications.Strategies;

public class DeliveryReviewNotificationStrategy : INotificationActionStrategy
{
    public NotificationEntityType EntityType => NotificationEntityType.DeliveryReview;

    public string? GetActionUrl(int? entityId)
    {
        if (!entityId.HasValue)
            return null;

        return $"/User/Profile/Reviews";
    }
}