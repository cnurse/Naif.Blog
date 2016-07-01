using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Naif.Blog.ViewComponents
{
    public class CopyrightViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
