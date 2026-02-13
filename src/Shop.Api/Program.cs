using Shop.Application.Categories;
using Shop.Infrastructure;
using Shop.Infrastructure.Categories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger UI (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI
builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.AddScoped<DanhMucService>();

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
