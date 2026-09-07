namespace FlowerShop.Application.Features.Notifications.Queries.GetNotifications;

public record GetNotificationsResponse
{
    public IReadOnlyList<NotificationDto> Notifications { get; init; } = [];
    public int UnreadCount { get; init; }
}