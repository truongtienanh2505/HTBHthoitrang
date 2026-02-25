using Shop.Application.Categories;
using Shop.Infrastructure;
using Shop.Infrastructure.Categories;
using System.Text.Json.Serialization;
using Shop.Application.Services;
using Shop.Infrastructure.Repositories;
using Shop.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Swagger UI (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<DanhMucService>();

builder.Services.AddScoped<PromotionService>();

builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
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
