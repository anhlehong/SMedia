using Microsoft.AspNetCore.Builder;
using SMedia.Middlewares;

namespace SMedia.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomHttpLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HttpLoggingMiddleware>();
        }
    }
}
