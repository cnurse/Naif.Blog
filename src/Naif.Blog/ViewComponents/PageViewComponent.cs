using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Models;

namespace Naif.Blog.ViewComponents
{
    public class PageViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Page page)
        {
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return View(page);
        }
        
    }
}