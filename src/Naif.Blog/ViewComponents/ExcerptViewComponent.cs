using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Models;
using System.Threading.Tasks;

#pragma warning disable 1998

namespace Naif.Blog.ViewComponents
{
    public class ExcerptViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(PostBase post)
        {
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return View(post);
        }
    }
}
