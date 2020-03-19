using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Services;

#pragma warning disable 1998

namespace Naif.Blog.ViewComponents
{
    public class GoogleAnalyticsViewComponent : BaseViewComponent
    {
        public GoogleAnalyticsViewComponent(IBlogRepository blogRepository, IApplicationContext appContext) : base(
            blogRepository, appContext)
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return View(Blog);
        }

    }
}
