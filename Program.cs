using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ParkingServer.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ParkingContext>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // token 验证配置
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!,
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")!,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")!))
    };
});
builder.Services.AddControllers();
builder.Services.AddAuthorization();

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
app.UseAuthentication();
app.UseAuthorization();

// 自动应用迁移
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ParkingContext>();
    db.Database.Migrate(); // 自动应用迁移

    // // 读取 user.json 文件并添加用户
    // var userJsonPath = Path.Combine(AppContext.BaseDirectory, "user.json");
    // if (File.Exists(userJsonPath))
    // {
    //     var jsonContent = File.ReadAllText(userJsonPath);
    //     using var jsonDoc = System.Text.Json.JsonDocument.Parse(jsonContent);
    //
    //     var usersAdded = 0;
    //     foreach (var element in jsonDoc.RootElement.EnumerateArray())
    //     {
    //         var account = element.GetProperty("account").GetString();
    //         var password = element.GetProperty("password").GetString();
    //
    //         if (account != null && password != null && !db.user.Any(u => u.account == account))
    //         {
    //             db.user.Add(new user
    //             {
    //                 account = account,
    //                 password = BCrypt.Net.BCrypt.HashPassword(password)
    //             });
    //             usersAdded++;
    //         }
    //     }
    //
    //     db.SaveChanges();
    //     Console.WriteLine($"成功添加 {usersAdded} 个用户到数据库");
    // }
    // else
    // {
    //     Console.WriteLine($"未找到 user.json 文件：{userJsonPath}");
    // }
}

Console.WriteLine("1010_01");

app.MapControllers();

app.Run();