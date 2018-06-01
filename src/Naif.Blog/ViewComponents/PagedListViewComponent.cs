using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Models;

namespace Naif.Blog.ViewComponents
{
    public class PagedListViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<Post> list, int pageCount)
        {
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return View(list.InPagesOf(pageCount).GetPage((int)ViewData["Page"]));
        }
    }
}
