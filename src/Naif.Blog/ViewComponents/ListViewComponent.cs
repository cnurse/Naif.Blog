using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

#pragma warning disable 1998

namespace Naif.Blog.ViewComponents
{
    public class ListViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<Post> list)
        {
            
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return View(list);
        }
    }
}


