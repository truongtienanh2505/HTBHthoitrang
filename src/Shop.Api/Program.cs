using Shop.Application.Categories;
using Shop.Infrastructure;
using Shop.Infrastructure.Categories;
using Shop.Application.Promotions;
using Shop.Api.BackgroundJobs;
using Shop.Application.Products;
using Shop.Application.AdminReports;
using Shop.Application.Vouchers;
using Shop.Api.Auth;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = DevHeaderAuthDefaults.Scheme;
        options.DefaultChallengeScheme = DevHeaderAuthDefaults.Scheme;
    })
    .AddScheme<AuthenticationSchemeOptions, DevHeaderAuthHandler>(
        DevHeaderAuthDefaults.Scheme,
        _ => { });

builder.Services.AddAuthorization();

// Swagger UI (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {c.AddSecurityDefinition("DevHeaderUserId", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
{
    Name = "X-Dev-UserId",
    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
    Description = "DEV only. Example: 1"
});

c.AddSecurityDefinition("DevHeaderRole", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
{
    Name = "X-Dev-Role",
    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
    Description = "DEV only. Example: Admin or User"
});

c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
{
    {
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Reference = new Microsoft.OpenApi.Models.OpenApiReference
            {
                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                Id = "DevHeaderUserId"
            }
        },
        Array.Empty<string>()
    },
    {
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Reference = new Microsoft.OpenApi.Models.OpenApiReference
            {
                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                Id = "DevHeaderRole"
            }
        },
        Array.Empty<string>()
    }
}); });
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


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
