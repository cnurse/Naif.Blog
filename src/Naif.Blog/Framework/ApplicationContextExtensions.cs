using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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