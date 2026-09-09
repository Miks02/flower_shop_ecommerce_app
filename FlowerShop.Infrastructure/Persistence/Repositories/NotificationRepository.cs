using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowerShop.Domain.Entities.Notifications;
using FlowerShop.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository(AppDbContext context) : Repository<Notification>(context), INotificationRepository
    {
        public async Task<IReadOnlyList<NotificationRecipient>> GetAllNotificationsByUserId(string userId)
        {
            return await Context.NotificationRecipients
                .Where(nt => nt.UserId == userId)
                .Include(nt => nt.Notification) 
                .ToListAsync();
        }

        public async Task<int> MarkNotificationsAsRead(string userId)
        {
            return await Context.NotificationRecipients
                .Where(nt => nt.UserId == userId) 
                .ExecuteUpdateAsync(nt => nt.SetProperty(x => x.ReadAt, DateTime.UtcNow));
        }

        public void AddNotificationWithMultipleRecipients(IReadOnlyList<string> userIds, Notification notification)
        {
            notification.Recipients = userIds.Select(userId => new NotificationRecipient
            {
                UserId = userId,
                Notification = notification

            }).ToList();

            Context.Add(notification);
        }

        public void Add(Notification notification, string userId)
        {
            notification.Recipients.Add(new NotificationRecipient
            {
                UserId = userId,
                Notification = notification
            });

            Context.Notifications.Add(notification);
        }

        public async Task<int> CountUnreadAsync(string userId, CancellationToken ct = default)
        {
            return await Context.NotificationRecipients
                .CountAsync(r => r.UserId == userId && r.ReadAt == null, ct);
        }
    }
}
