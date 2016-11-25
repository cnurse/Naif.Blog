using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Naif.Blog.Framework
{
	public class ThemeViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var enumerable = viewLocations as string[] ?? viewLocations.ToArray();
            var themeLocations = enumerable.ToList();
            if (context.Values.ContainsKey("theme"))
            {
                themeLocations.InsertRange(0, enumerable.Select(f => f.Replace("/Views/", "/Views/" + context.Values["theme"] + "/")));
            }
            return themeLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var appContext = context.ActionContext.HttpContext.RequestServices
                        .GetService(typeof(IApplicationContext)) as IApplicationContext;

            if (appContext != null && !string.IsNullOrEmpty(appContext.CurrentBlog.Theme))
            {
                context.Values["theme"] = appContext.CurrentBlog.Theme;
            }
        }
    }
}

