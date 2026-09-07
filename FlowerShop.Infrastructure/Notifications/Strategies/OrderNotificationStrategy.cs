using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Notifications;

namespace FlowerShop.Infrastructure.Notifications.Strategies;

public class OrderNotificationStrategy(IUserProvider userProvider) : INotificationActionStrategy
{
    public NotificationEntityType EntityType => NotificationEntityType.Order;

    public string? GetActionUrl(int? entityId)
    {
        if (!entityId.HasValue)
            return "/User/Orders/";
        
        if(userProvider.IsAdmin())
            return $"/Admin/Orders/Details/{entityId.Value}";
        if(userProvider.IsUser())
            return "/User/Orders/";
        if(userProvider.IsDeliverer())
            return $"/Deliverer/Orders/Details/{entityId.Value}";

        return null;
    }
}