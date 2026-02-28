using Shop.Application.Categories;
using Shop.Infrastructure;
using Shop.Infrastructure.Categories;
using Shop.Application.Promotions;
using Shop.Api.BackgroundJobs;
using Shop.Application.Products;
using Shop.Application.AdminReports;
using Shop.Application.Vouchers;
using Shop.Api.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddAuthentication(DevHeaderAuthDefaults.Scheme)
    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, DevHeaderAuthHandler>(
        DevHeaderAuthDefaults.Scheme,
        _ => { });

builder.Services.AddAuthorization();

// Swagger UI (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<PromotionCacheService>();


// DI
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<DanhMucService>();

builder.Services.Configure<PromotionCacheOptions>(builder.Configuration.GetSection("PromotionCache"));
builder.Services.AddHostedService<PromotionCacheHostedService>();
builder.Services.AddScoped<ProductQueryService>();

builder.Services.AddScoped<AdminReportService>();

builder.Services.AddScoped<VoucherService>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
