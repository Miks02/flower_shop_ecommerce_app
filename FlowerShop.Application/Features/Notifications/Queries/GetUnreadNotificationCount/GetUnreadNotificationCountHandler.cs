using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Notifications;

namespace FlowerShop.Application.Features.Notifications.Queries.GetUnreadNotificationCount;

public class GetUnreadNotificationCountHandler(INotificationRepository notificationRepo) : IHandler
{
    public async Task<int> Handle(GetUnreadNotificationCountQuery request, CancellationToken ct = default)
    {
        return await notificationRepo.CountUnreadAsync(request.UserId, ct);
    }
}
