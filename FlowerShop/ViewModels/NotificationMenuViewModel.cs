using FlowerShop.Domain.Entities.Notifications;

namespace FlowerShop.Web.ViewModels;

public record NotificationMenuViewModel
{
    public IReadOnlyList<NotificationViewModel> Notifications { get; init; } = [];
    public int UnreadCount { get; init; }
}