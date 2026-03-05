using Shop.Application.Categories;
using Shop.Infrastructure;
using Shop.Infrastructure.Categories;

var builder = WebApplication.CreateBuilder(args);
// Cấp quyền CORS cho phép mọi Frontend đều được gọi vào Backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddControllers();

// Swagger UI (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<DanhMucService>();

var app = builder.Build();
// Kích hoạt CORS (Phải đặt trước các Use khác như UseAuthorization)
app.UseCors("AllowAll");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
