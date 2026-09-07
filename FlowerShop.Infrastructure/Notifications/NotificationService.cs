using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.IdentityUser;
using FlowerShop.Domain.Entities.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Notifications
{
    public class NotificationService(
        INotificationRepository notificationRepo,
        UserManager<User> userManager,
        IUnitOfWork unitOfWork) : INotificationService
    {
        public async Task SendNotificationAsync(
            string userId, 
            string title, 
            string message,
            NotificationType type = NotificationType.Information,
            NotificationEntityType entityType = NotificationEntityType.None)
        {
            var recipient = await userManager.Users.AnyAsync(u => u.Id == userId);

            if(!recipient)
                throw new NotificationException($"Recipient with ID: {userId} has not been found.");

            var newNotification = new Notification
            {
                Title = title,
                Message = message,
                NotificationType = type,
                NotificationEntityType = entityType
            };

            notificationRepo.Add(newNotification, userId);
            await unitOfWork.SaveAsync();
        }
        public async Task SendMultipleNotificationsAsync(
            IReadOnlyList<string> userIds, 
            string title,
            string message,
            NotificationType type = NotificationType.Information,
            NotificationEntityType entityType = NotificationEntityType.None)
        {
            var usersExists = await userManager.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();

            if (usersExists.Count != userIds.Count)
                throw new NotificationException("Some recipients have not been found.");

            var newNotification = new Notification
            {
                Title = title,
                Message = message,
                NotificationType = type,
                NotificationEntityType = entityType
            };

            notificationRepo.AddNotificationWithMultipleRecipients(userIds, newNotification);
            await unitOfWork.SaveAsync();
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
           await notificationRepo.MarkNotificationsAsRead(userId);
        }

        public async Task SendNotificationsToAllAdminsAsync(
            string title,
            string message,
            NotificationType type = NotificationType.Information,
            NotificationEntityType entityType = NotificationEntityType.None)
        {
            var users = await userManager.GetUsersInRoleAsync("Admin");

            if(users.Count == 0)
                throw new NotificationException("No admin users found.");

            var newNotification = new Notification
            {
                Title = title,
                Message = message,
                NotificationType = type,
                NotificationEntityType = entityType
            };

            notificationRepo.AddNotificationWithMultipleRecipients(users.Select(u => u.Id).ToList(), newNotification);
            await unitOfWork.SaveAsync();
        }

        public async Task SendNotificationsToAllDeliverersAsync(
            string title,
            string message,
            NotificationType type = NotificationType.Information,
            NotificationEntityType entityType = NotificationEntityType.None)
        {
            var users = await userManager.GetUsersInRoleAsync("Deliverer");

            if (users.Count == 0)
                throw new NotificationException("No deliverer users found.");

            var newNotification = new Notification
            {
                Title = title,
                Message = message,
                NotificationType = type,
                NotificationEntityType = entityType
            };

            notificationRepo.AddNotificationWithMultipleRecipients(users.Select(u => u.Id).ToList(), newNotification);
            await unitOfWork.SaveAsync();
        }

        public async Task SendNotificationsToAllUsersAsync(
            string title,
            string message,
            NotificationType type = NotificationType.Information,
            NotificationEntityType entityType = NotificationEntityType.None)
        {
            var users = await userManager.GetUsersInRoleAsync("User");

            if (users.Count == 0)
                throw new NotificationException("No user users found.");

            var newNotification = new Notification
            {
                Title = title,
                Message = message,
                NotificationType = type,
                NotificationEntityType = entityType
            };

            notificationRepo.AddNotificationWithMultipleRecipients(users.Select(u => u.Id).ToList(), newNotification);
            await unitOfWork.SaveAsync();
        }
    }
}
