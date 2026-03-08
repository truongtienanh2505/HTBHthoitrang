using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop.Application.AdminReports;
using Shop.Application.Articles;
using Shop.Application.Auth;
using Shop.Application.Banners;
using Shop.Application.Categories;
using Shop.Application.Products;
using Shop.Application.Promotions;
using Shop.Application.Reviews;
using Shop.Application.UserAddresses;
using Shop.Application.Vouchers;
using Shop.Infrastructure.AdminReports;
using Shop.Infrastructure.Articles;
using Shop.Infrastructure.Auth;
using Shop.Infrastructure.Banners;
using Shop.Infrastructure.Categories;
using Shop.Infrastructure.Orders;
using Shop.Infrastructure.Persistence;
using Shop.Infrastructure.Products;
using Shop.Infrastructure.Promotions;
using Shop.Infrastructure.Reviews;
using Shop.Infrastructure.Search;
using Shop.Infrastructure.UserAddresses;
using Shop.Infrastructure.Vouchers;

namespace Shop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ShopDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped<IDanhMucRepository, DanhMucRepository>();
        services.AddScoped<IPromotionCacheRepository, PromotionCacheRepository>();
        services.AddScoped<IProductQueryRepository, ProductQueryRepository>();
        services.AddScoped<IAdminReportRepository, AdminReportRepository>();
        services.AddScoped<IVoucherRepository, VoucherRepository>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<SearchService>();
        services.AddScoped<OrderService>();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IUserAddressRepository, UserAddressRepository>();
        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<IBannerRepository, BannerRepository>();

        return services;
    }
}