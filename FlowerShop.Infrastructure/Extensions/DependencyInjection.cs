using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Infrastructure.Identity;
using FlowerShop.Infrastructure.Persistence.EntityFramework;
using FlowerShop.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlowerShop.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddIdentity();
        services.AddHandlers();
        services.AddInfrastructureServices();
    }

    public static void AddHandlers(this IServiceCollection services)
    {
        var handlers = typeof(Application.AssemblyReference).Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(IHandler)));

        foreach (var handler in handlers)
            services.AddScoped(handler);
        
    }

    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IFileService, LocalFileStorage>();
        services.AddScoped<IUserProvider, UserProvider>();
    }
}