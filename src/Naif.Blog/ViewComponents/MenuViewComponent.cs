using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Models;

#pragma warning disable 1998

namespace Naif.Blog.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Menu menu)
        {
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return View(menu);
        }
    }
}