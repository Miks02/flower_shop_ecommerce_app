using FlowerShop.Infrastructure.ExceptionHandling;
using Microsoft.Extensions.DependencyInjection;

namespace FlowerShop.Infrastructure.Extensions;

public static class ExceptionHandlingRegistration
{
    public static void AddExceptionHandlers(this IServiceCollection services)
    {
        services.AddExceptionHandler<DbUpdateExceptionHandler>();
        services.AddExceptionHandler<NotificationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
    }
}
