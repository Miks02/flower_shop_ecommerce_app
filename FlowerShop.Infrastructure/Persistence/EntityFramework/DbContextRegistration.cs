using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Categories;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Domain.Entities.Products;
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
    }
}