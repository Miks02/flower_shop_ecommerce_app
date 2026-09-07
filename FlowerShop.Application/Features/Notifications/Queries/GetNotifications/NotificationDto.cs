using FlowerShop.Domain.Entities.Notifications;

namespace FlowerShop.Application.Features.Notifications.Queries.GetNotifications;

public record NotificationDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string Message { get; init; } = null!;
    public NotificationType NotificationType { get; init; }
    public NotificationEntityType EntityType { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsRead { get; init; }
    public string? ActionUrl { get; init; }
    public string IconClass { get; init; } = null!;
    public string ColorClass { get; init; } = null!;
    public string BackgroundColorClass { get; init; } = null!;
}