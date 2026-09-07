using FlowerShop.Domain.Entities.IdentityUser;

namespace FlowerShop.Domain.Entities.Notifications;

public class Notification
{
    public int Id { get; set; }
    public NotificationType NotificationType { get; set; } = NotificationType.Information;
    public NotificationEntityType NotificationEntityType { get; set; } = NotificationEntityType.None;
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<NotificationRecipient> Recipients { get; set; } = [];
}