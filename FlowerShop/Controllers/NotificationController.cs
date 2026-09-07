using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Application.Features.Notifications.Queries.GetNotifications;
using FlowerShop.Web.ViewModels;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Controllers;

[Authorize]
public class NotificationController(
    IUserProvider userProvider,
    GetNotificationsHandler getNotificationsHandler,
    INotificationService notificationService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetNotifications(CancellationToken ct = default)
    {
        var userId = userProvider.GetCurrentUserId();
        var response = await getNotificationsHandler.Handle(new GetNotificationsQuery { UserId = userId }, ct);
        
        var vm = new NotificationMenuViewModel
        {
            Notifications = response.Notifications.Select(n => new NotificationViewModel
            {
                Id = n.Id,
                ActionUrl = n.ActionUrl,
                ColorClass = n.ColorClass,
                CreatedAt = n.CreatedAt,
                IconClass = n.IconClass,
                IsRead = n.IsRead,
                Message = n.Message,
                NotificationType = n.NotificationType,
                EntityType = n.EntityType,
                Title = n.Title,
                BackgroundColorClass = n.BackgroundColorClass
            })
            .ToList(),
            UnreadCount = response.UnreadCount
        };  

        return PartialView("_NotificationMenu", vm);
    }

    [HttpPost]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken ct = default)
    {
        var userId = userProvider.GetCurrentUserId();
        await notificationService.MarkAllAsReadAsync(userId);
        
        if (Request.IsHtmx())
        {
            var response = await getNotificationsHandler.Handle(new GetNotificationsQuery { UserId = userId }, ct);
            
            var vm = new NotificationMenuViewModel
            {
                Notifications = response.Notifications.Select(n => new NotificationViewModel
                    {
                        Id = n.Id,
                        ActionUrl = n.ActionUrl,
                        ColorClass = n.ColorClass,
                        CreatedAt = n.CreatedAt,
                        IconClass = n.IconClass,
                        IsRead = n.IsRead,
                        Message = n.Message,
                        NotificationType = n.NotificationType,
                        EntityType = n.EntityType,
                        Title = n.Title,
                        BackgroundColorClass = n.BackgroundColorClass
                    })
                    .ToList(),
                UnreadCount = response.UnreadCount
            };  
            return PartialView("_NotificationMenu", vm);
        }

        return RedirectToAction("Index", "Home");
    }
}