using System.Net.WebSockets;
using DotNetEnv;
using SMedia.Configuration;
using Serilog;
using SMedia.Extensions;
using Microsoft.AspNetCore.Http;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Tắt tất cả logger mặc định
builder.Logging.ClearProviders();

// Cấu hình Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        // .WriteTo.File("logs/log-.txt",
        //     rollingInterval: RollingInterval.Day,
        //     outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.SignalR", Serilog.Events.LogEventLevel.Debug)
        .MinimumLevel.Override("Microsoft.AspNetCore.Http.Connections", Serilog.Events.LogEventLevel.Debug)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Fatal)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Fatal)
        .Enrich.FromLogContext();
});

// Thêm các dịch vụ ứng dụng
builder.Services.AddApplicationServices();
builder.Services.AddRealtimeServices();

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(Env.GetString("FQDN_FRONTEND", "http://localhost:3000"))
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SMedia API V1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "SMedia API Documentation";
        options.DefaultModelsExpandDepth(-1);
        options.DisplayRequestDuration();
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}

// Đăng ký middleware
app.UseCors("AllowFrontend");
app.UseCustomHttpLogging();
app.UseCustomHttpLogging();

app.UseAuthentication();
app.UseWebSockets();
app.UseRealtimeHandler();

app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();