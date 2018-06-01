using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Models;
using System.Threading.Tasks;

namespace Naif.Blog.ViewComponents
{
	public class PostViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Post post)
        {
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return View(post);
        }
    }
}
