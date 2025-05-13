using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.DTOs;
using DotNetEnv;
using Infrastructure.Data;
using Application.Interfaces.ServiceInterfaces;
using Application.Services;
using Application.Interfaces.RepositoryInterfaces;
using Infrastructure.Repositories;
using Mapster;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.DocInclusionPredicate((_, _) => true);
    options.CustomOperationIds(e => e.ActionDescriptor.RouteValues["action"]);
    options.OperationFilter<RemoveDefaultResponse>();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập token JWT: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Đọc cấu hình JWT từ .env
var jwtConfig = new JwtConfiguration
{
    Issuer = Env.GetString("JWT_ISSUER") ?? throw new InvalidOperationException("JWT_ISSUER is not set in .env"),
    Audience = Env.GetString("JWT_AUDIENCE") ?? throw new InvalidOperationException("JWT_AUDIENCE is not set in .env"),
    Key = Env.GetString("JWT_KEY") ?? throw new InvalidOperationException("JWT_KEY is not set in .env")
};

// Đăng ký JwtConfiguration như một singleton
builder.Services.AddSingleton(jwtConfig);

// Cấu hình JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(Env.GetString("CONNECTION_STRING")));

builder.Services.AddMemoryCache();

// Đọc cấu hình email từ .env
var emailConfig = new EmailConfiguration
{
    SmtpServer = Env.GetString("EMAIL_SMTP_SERVER") ?? throw new InvalidOperationException("EMAIL_SMTP_SERVER is not set in .env"),
    Port = int.TryParse(Env.GetString("EMAIL_PORT"), out var port) ? port : throw new InvalidOperationException("EMAIL_PORT is not set or invalid in .env"),
    SenderEmail = Env.GetString("EMAIL_SENDER_EMAIL") ?? throw new InvalidOperationException("EMAIL_SENDER_EMAIL is not set in .env"),
    Password = Env.GetString("EMAIL_PASSWORD") ?? throw new InvalidOperationException("EMAIL_PASSWORD is not set in .env"),
    SenderName = Env.GetString("EMAIL_SENDER_NAME") ?? throw new InvalidOperationException("EMAIL_SENDER_NAME is not set in .env")
};

// Đăng ký EmailConfiguration như một singleton
builder.Services.AddSingleton(emailConfig);

// Đăng ký Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Đăng ký Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Thêm dịch vụ Mapster
builder.Services.AddMapster();
// Gọi cấu hình ánh xạ
MapsterConfig.RegisterMappings();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();