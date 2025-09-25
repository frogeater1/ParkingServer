using Microsoft.EntityFrameworkCore;
using ParkingServer.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ParkingContext>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();
app.UseCors("CorsPolicy");

// app.UseAuthorization();
// 自动应用迁移
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ParkingContext>();
    dbContext.Database.Migrate(); // 自动应用迁移
}

app.MapControllers();

app.Run();