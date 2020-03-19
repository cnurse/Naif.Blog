using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable 1998

namespace Naif.Blog.ViewComponents
{
    public class ClientDependencyViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return View();
        }
    }
}