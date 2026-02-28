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
using Shop.Application.AdminReports;
using Shop.Infrastructure.AdminReports;
using Shop.Application.Vouchers;
using Shop.Infrastructure.Vouchers;

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
        services.AddScoped<IAdminReportRepository, AdminReportRepository>();
        services.AddScoped<IVoucherRepository, VoucherRepository>();



        return services;
    }
}
