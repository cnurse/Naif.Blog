using Microsoft.AspNet.Mvc;
using Naif.Blog.Services;

namespace Naif.Blog.Controllers
{
    public class BlogController : BaseController
    {

        public BlogController(IBlogRepository blogRepository) : base(blogRepository) { }

        public IActionResult Index()
        {
            return View(BlogRepository.GetAll());
        }
    }
}
