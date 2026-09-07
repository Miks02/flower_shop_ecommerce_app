namespace FlowerShop.Application.Features.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery
{
    public string UserId { get; init; } = null!;
}