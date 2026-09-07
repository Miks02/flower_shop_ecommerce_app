using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Notifications;

namespace FlowerShop.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsHandler(
    INotificationRepository notificationRepo,
    INotificationResolver notificationResolver) : IHandler
{
    public async Task<GetNotificationsResponse> Handle(GetNotificationsQuery request, CancellationToken ct = default)
    {
        var notifications = await notificationRepo.GetAllNotificationsByUserId(request.UserId);

        var dtos = notifications.Select(n => new NotificationDto
        {
            Id = n.Notification.Id,
            Title = n.Notification.Title,
            Message = n.Notification.Message,
            NotificationType = n.Notification.NotificationType,
            EntityType = n.Notification.NotificationEntityType,
            CreatedAt = n.Notification.CreatedAt,
            IsRead = n.ReadAt != null,
            ActionUrl = notificationResolver.GetActionUrl(n.Notification.NotificationEntityType, n.Notification.EntityId),
            IconClass = notificationResolver.GetIconClass(n.Notification.NotificationType),
            ColorClass = notificationResolver.GetColorClass(n.Notification.NotificationType),
            BackgroundColorClass = notificationResolver.GetBackgroundColorClass(n.Notification.NotificationType)
        }).OrderByDescending(n => n.CreatedAt).ToList();

        var unreadCount = dtos.Count(n => !n.IsRead);

        return new GetNotificationsResponse
        {
            Notifications = dtos,
            UnreadCount = unreadCount
        };
    }
}