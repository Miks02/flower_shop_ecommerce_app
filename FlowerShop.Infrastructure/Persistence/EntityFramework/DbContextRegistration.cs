using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Carts;
using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Entities.LoyaltyTransactions;
using FlowerShop.Domain.Entities.Notifications;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Domain.Entities.Orders;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.Domain.Entities.Reviews;
using FlowerShop.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlowerShop.Infrastructure.Persistence.EntityFramework;

public static class DbContextRegistration
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IFlowerRepository, FlowerRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IOccasionRepository, OccasionRepository>();
        services.AddScoped<IDelivererRepository, DelivererRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ILoyaltyTransactionRepository, LoyaltyTransactionRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
    }
}