using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlowerShop.Domain.Entities.Notifications
{
    public interface INotificationRepository
    {
        void Add(Notification notification, string userId);
        void Update(Notification notification);
        void Remove (Notification notification);
        Task<IReadOnlyList<NotificationRecipient>> GetAllNotificationsByUserId(string userId);
        Task<int> MarkNotificationsAsRead(string userId);
        void AddNotificationWithMultipleRecipients(IReadOnlyList<string> userIds, Notification notification);
        Task<int> CountUnreadAsync(string userId, CancellationToken ct = default);
    }
}
