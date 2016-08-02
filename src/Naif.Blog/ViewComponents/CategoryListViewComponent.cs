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
            return View(BlogRepository.GetCategories(Blog.Id));
        }
    }
}
