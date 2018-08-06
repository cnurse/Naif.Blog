using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Models;

namespace Naif.Blog.ViewComponents
{
    public class PagerViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Pager pager)
        {
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return View(pager);
        }
    }
}