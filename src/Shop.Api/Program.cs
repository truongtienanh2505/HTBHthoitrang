using Shop.Application.Categories;
using Shop.Infrastructure;
using Shop.Infrastructure.Categories;
using Shop.Application.Promotions;
using Shop.Api.BackgroundJobs;
using Shop.Application.Products;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
