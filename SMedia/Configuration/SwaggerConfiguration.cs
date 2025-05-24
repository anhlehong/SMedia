using Application.DTOs;
using Microsoft.OpenApi.Models;

namespace SMedia.Configuration;


public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.DocInclusionPredicate((_, _) => true);
            options.CustomOperationIds(e => e.ActionDescriptor.RouteValues["action"]);
            options.OperationFilter<RemoveDefaultResponse>();

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
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

        return services;
    }
}