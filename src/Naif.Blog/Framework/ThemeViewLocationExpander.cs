using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;
using Naif.Blog.Models;
using Microsoft.Extensions.Options;

namespace Naif.Blog.Framework
{
	public class ThemeViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var themeLocations = viewLocations.ToList();
            if (context.Values.ContainsKey("theme"))
            {
                themeLocations.InsertRange(0, viewLocations.Select(f => {
                        return f.Replace("/Views/", "/Views/" + context.Values["theme"] + "/");
                    }));
            }
            return themeLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var optionsAccessor = context.ActionContext.HttpContext.RequestServices
                        .GetService(typeof(IOptions<BlogOptions>)) as IOptions<BlogOptions>;
            var options = optionsAccessor.Value;

            if (!string.IsNullOrEmpty(options.Theme))
            {
                context.Values["theme"] = options.Theme;
            }
        }
    }
}

