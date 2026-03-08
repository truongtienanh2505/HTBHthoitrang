using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shop.Api.Auth;
using Shop.Api.BackgroundJobs;
using Shop.Application.AdminReports;
using Shop.Application.Articles;
using Shop.Application.Banners;
using Shop.Application.Categories;
using Shop.Application.Products;
using Shop.Application.Promotions;
using Shop.Application.Reviews;
using Shop.Application.UserAddresses;
using Shop.Application.Vouchers;
using Shop.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtSecret = builder.Configuration["JwtSettings:Secret"]
    ?? throw new InvalidOperationException("Thiếu JwtSettings:Secret trong appsettings.json");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Giữ nguyên PascalCase để mấy file JS hiện tại đỡ lệch key
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "SmartScheme";
        options.DefaultChallengeScheme = "SmartScheme";
    })
    .AddPolicyScheme("SmartScheme", "JWT or DevHeader", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrWhiteSpace(authHeader) &&
                authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return JwtBearerDefaults.AuthenticationScheme;
            }

            if (context.Request.Headers.ContainsKey(DevHeaderAuthDefaults.HeaderUserId))
            {
                return DevHeaderAuthDefaults.Scheme;
            }

            return JwtBearerDefaults.AuthenticationScheme;
        };
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    })
    .AddScheme<AuthenticationSchemeOptions, DevHeaderAuthHandler>(
        DevHeaderAuthDefaults.Scheme,
        _ => { });

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Shop API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập theo dạng: Bearer {token}"
    });

    c.AddSecurityDefinition("DevHeaderUserId", new OpenApiSecurityScheme
    {
        Name = "X-Dev-UserId",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "DEV only. Example: 1"
    });

    c.AddSecurityDefinition("DevHeaderRole", new OpenApiSecurityScheme
    {
        Name = "X-Dev-Role",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "DEV only. Example: Admin hoặc User"
    });
});

// Infrastructure + repositories
builder.Services.AddInfrastructure(builder.Configuration);

// Application services
builder.Services.AddScoped<DanhMucService>();
builder.Services.AddScoped<PromotionCacheService>();
builder.Services.AddScoped<ProductQueryService>();
builder.Services.AddScoped<AdminReportService>();
builder.Services.AddScoped<VoucherService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<UserAddressService>();
builder.Services.AddScoped<ArticleService>();
builder.Services.AddScoped<BannerService>();

builder.Services.Configure<PromotionCacheOptions>(
    builder.Configuration.GetSection("PromotionCache"));
builder.Services.AddHostedService<PromotionCacheHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();