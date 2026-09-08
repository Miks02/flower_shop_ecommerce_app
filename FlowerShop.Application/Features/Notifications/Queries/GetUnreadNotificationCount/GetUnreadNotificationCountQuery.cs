namespace FlowerShop.Application.Features.Notifications.Queries.GetUnreadNotificationCount;

public record GetUnreadNotificationCountQuery
{
    public string UserId { get; init; } = null!;
}
