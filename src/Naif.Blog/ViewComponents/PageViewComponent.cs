using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Models;

#pragma warning disable 1998

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