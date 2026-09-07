using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowerShop.Domain.Entities.Notifications;

namespace FlowerShop.Application.Common.Abstractions
{
    public interface INotificationService
    {
        Task SendNotificationAsync(
            string userId,
            string title,
            string message,
            NotificationType type = NotificationType.Information,
            NotificationEntityType entityType = NotificationEntityType.None);
        Task SendMultipleNotificationsAsync(
            IReadOnlyList<string> userIds, 
            string title,
            string message,
            NotificationType type = NotificationType.Information,
            NotificationEntityType entityType = NotificationEntityType.None);

        Task SendNotificationsToAllAdminsAsync(
            string title,
            string message,
            NotificationType type = NotificationType.Information,
            NotificationEntityType entityType = NotificationEntityType.None);

        Task SendNotificationsToAllDeliverersAsync(
            string title,
            string message,
            NotificationType type = NotificationType.Information,
            NotificationEntityType entityType = NotificationEntityType.None);

        Task SendNotificationsToAllUsersAsync(
            string title,
            string message,
            NotificationType type = NotificationType.Information,
            NotificationEntityType entityType = NotificationEntityType.None);
        Task MarkAllAsReadAsync(string userId); 

    }
}
