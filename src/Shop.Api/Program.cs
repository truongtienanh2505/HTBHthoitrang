using Shop.Application.Reviews;
using Shop.Infrastructure.Reviews;
using Shop.Application.Products;
using Shop.Infrastructure.Products;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Shop.Application.Categories;
using Shop.Infrastructure;
using Shop.Infrastructure.Categories;
using Shop.Application.UserAddresses;
using Shop.Infrastructure.UserAddresses;
using Shop.Application.Articles;
using Shop.Infrastructure.Articles;
using Shop.Application.Banners;
using Shop.Infrastructure.Banners;
var builder = WebApplication.CreateBuilder(args);
// Cấp quyền CORS cho phép mọi Frontend đều được gọi vào Backend
builder.Services.AddScoped<IUserAddressRepository, UserAddressRepository>();
builder.Services.AddScoped<UserAddressService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ArticleService>();
builder.Services.AddScoped<IBannerRepository, BannerRepository>();
builder.Services.AddScoped<BannerService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddControllers().AddJsonOptions(options =>
    {
        // Ép C# giữ nguyên chữ viết Hoa (PascalCase) khi gửi xuống file JS
        options.JsonSerializerOptions.PropertyNamingPolicy = null; 
    });;

// Swagger UI (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<DanhMucService>();
// Cấu hình giải mã JWT Token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
        };
    });

builder.Services.AddAuthorization();
var app = builder.Build();
// Kích hoạt CORS (Phải đặt trước các Use khác như UseAuthorization)
app.UseCors("AllowAll");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
