using DotNetEnv;
using SMedia.Configuration;
using Serilog;
using SMedia.Extensions;
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
        .WriteTo.File("logs/log-.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Fatal)
        .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Fatal)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Fatal)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Fatal)
        .Enrich.FromLogContext();
});

// Tắt log EF Core chi tiết
builder.Services.AddLogging(logging =>
{
    logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
    logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.None);
    logging.AddFilter("Microsoft.AspNetCore", LogLevel.None);
    logging.AddFilter("System", LogLevel.None);
});


builder.Services.AddApplicationServices();
// builder.Services.AddWebSocketServices();

//Cho phép website khác truy cập vào API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(Env.GetString("FQDN_FRONTEND"))
                  .AllowAnyHeader()
                  .AllowAnyMethod();
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
        options.DefaultModelsExpandDepth(-1); // Hide schemas section by default
        options.DisplayRequestDuration();
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Collapse operations by default
    });
}

app.UseCustomHttpLogging();
app.UseWebSockets(); // Kích hoạt WebSocket middleware
// app.UseWebSocketHandler(); // Xử lý các yêu cầu WebSocket tại /ws

// Đăng ký middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();