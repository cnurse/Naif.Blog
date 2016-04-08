using Microsoft.AspNet.Mvc;

namespace Naif.Blog.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
