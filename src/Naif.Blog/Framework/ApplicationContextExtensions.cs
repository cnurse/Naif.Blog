using Microsoft.AspNetCore.Builder;

namespace Naif.Blog.Framework
{
    public static class ApplicationContextExtensions
    {
        public static IApplicationBuilder UseApplicationContext(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ApplicationContextMiddleware>();
        }
    }
}