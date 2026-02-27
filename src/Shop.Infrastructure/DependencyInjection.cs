using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop.Application.Categories;
using Shop.Infrastructure.Categories;
using Shop.Infrastructure.Persistence;
using Shop.Application.Promotions;
using Shop.Infrastructure.Promotions;
using Shop.Application.Products;
using Shop.Infrastructure.Products;

namespace Shop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ShopDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped<IDanhMucRepository, DanhMucRepository>();
        services.AddScoped<IPromotionCacheRepository, PromotionCacheRepository>();
        services.AddScoped<IProductQueryRepository, ProductQueryRepository>();



        return services;
    }
}
