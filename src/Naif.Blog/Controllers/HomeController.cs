using Microsoft.AspNet.Mvc;

namespace Angular2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
