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
            NotificationEntityType entityType = NotificationEntityType.None,
            int? entityId = null);
        Task SendMultipleNotificationsAsync(
            IReadOnlyList<string> userIds, 
            string title,
            string message,
            NotificationType type = NotificationType.Information,
            NotificationEntityType entityType = NotificationEntityType.None,
            int? entityId = null);

        Task SendNotificationsToAllAdminsAsync(
            string title,
            string message,
            NotificationType type = NotificationType.Information,
            NotificationEntityType entityType = NotificationEntityType.None,
            int? entityId = null);

        Task SendNotificationsToAllDeliverersAsync(
            string title,
            string message,
            NotificationType type = NotificationType.Information,
            NotificationEntityType entityType = NotificationEntityType.None,
            int? entityId = null);

        Task SendNotificationsToAllUsersAsync(
            string title,
            string message,
            NotificationType type = NotificationType.Information,
            NotificationEntityType entityType = NotificationEntityType.None,
            int? entityId = null);
        Task MarkAllAsReadAsync(string userId); 

    }
}
