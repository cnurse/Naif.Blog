using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Services;
using System.Threading.Tasks;

#pragma warning disable 1998

namespace Naif.Blog.ViewComponents
{
    public class CopyrightViewComponent : BaseViewComponent
    {
        public CopyrightViewComponent(IBlogRepository blogRepository, IApplicationContext appContext)
            : base(blogRepository, appContext) { }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return View(Blog);
        }
    }
}
