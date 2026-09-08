using FlowerShop.Domain.Entities.Notifications;

namespace FlowerShop.Infrastructure.Notifications.Strategies;

public class ProductReviewNotificationStrategy : INotificationActionStrategy
{
    public NotificationEntityType EntityType => NotificationEntityType.ProductReview;

    public string? GetActionUrl(int? entityId)
    {
        if (!entityId.HasValue)
            return null;

        return $"/Catalogue/Details/{entityId.Value}";
    }
}
