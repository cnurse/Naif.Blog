using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Services;
using System.Threading.Tasks;

namespace Naif.Blog.ViewComponents
{
    public class CopyrightViewComponent : BaseViewComponent
    {
        public CopyrightViewComponent(IBlogRepository blogRepository, IApplicationContext appContext)
            : base(blogRepository, appContext) { }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(Blog);
        }
    }
}
