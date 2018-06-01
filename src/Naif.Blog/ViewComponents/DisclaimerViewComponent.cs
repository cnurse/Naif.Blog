using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Services;
using System.Threading.Tasks;

namespace Naif.Blog.ViewComponents
{
    public class DisclaimerViewComponent : BaseViewComponent
    {
        public DisclaimerViewComponent(IBlogRepository blogRepository, IApplicationContext appContext)
            : base(blogRepository, appContext) { }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return View(Blog);
        }
    }
}
