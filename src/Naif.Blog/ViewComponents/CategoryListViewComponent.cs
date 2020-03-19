using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Services;
using Naif.Blog.Framework;
using System.Threading.Tasks;

namespace Naif.Blog.ViewComponents
{
    public class CategoryListViewComponent : BaseViewComponent
    {
        public CategoryListViewComponent(IBlogRepository blogRepository, IApplicationContext appContext)
            : base(blogRepository, appContext) { }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, int> model = null;
			
            await Task.Run(() =>
            {
                model = BlogRepository.GetCategories(Blog.Id);
            });

            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return View(model);
        }
    }
}
