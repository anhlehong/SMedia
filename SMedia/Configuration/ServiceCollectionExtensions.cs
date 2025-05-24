using Application.Interfaces.RepositoryInterfaces;
using Mapster;
using Application.Interfaces.ServiceInterfaces;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace SMedia.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Thêm controllers
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        // Cấu hình Swagger
        services.AddSwaggerConfiguration();

        // Cấu hình xác thực JWT
        services.AddJwtAuthentication();

        // Thêm phân quyền
        services.AddAuthorization();

        // Cấu hình DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Env.GetString("CONNECTION_STRING")));

        // Thêm bộ nhớ cache
        services.AddMemoryCache();

        // Cấu hình email
        services.AddEmailConfiguration();

        // Đăng ký các dịch vụ
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IFollowService, FollowService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<ICommentService, CommentService>();
            
        // Đăng ký các kho lưu trữ
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFollowRepository, FollowRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        
        // Thêm Mapster
        services.AddMapster();
        MapsterConfig.RegisterMappings();

        return services;
    }
}