using Microsoft.AspNet.Mvc;
using Naif.Blog.Services;

namespace Naif.Blog.Controllers
{
    public class BlogController : Controller
    {
        private IBlogRepository _blogRepository;

        public BlogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public IActionResult Index()
        {
            return View(_blogRepository.GetAll());
        }
    }
}
