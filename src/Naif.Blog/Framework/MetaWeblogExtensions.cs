using Microsoft.AspNetCore.Builder;

namespace Naif.Blog.Framework
{
    public static class MetaWeblogExtensions
    {
        public static IApplicationBuilder UseMetaWeblog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<MetaWeblogMiddleware>();
        }
    }
}