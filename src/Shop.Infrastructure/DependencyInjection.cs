using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop.Application.Categories;
using Shop.Infrastructure.Categories;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ShopDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped<IDanhMucRepository, DanhMucRepository>();

        return services;
    }
}
