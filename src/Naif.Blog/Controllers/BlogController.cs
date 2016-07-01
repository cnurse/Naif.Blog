using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Services;
using System.Linq;

namespace Naif.Blog.Controllers
{
	public class BlogController : BaseController
    {

        public BlogController(IBlogRepository blogRepository) : base(blogRepository) { }

        public IActionResult Index()
        {
            return View("Index", BlogRepository.GetAll());
        }

        public IActionResult ViewCategory(string category)
        {
            return View("Index", BlogRepository.GetAll().Where(p => p.Categories.Contains(category)));
        }

        public IActionResult ViewPost(string slug)
        {
            return View(BlogRepository.GetAll().Single(p => p.Slug == slug));
        }

        public IActionResult ViewTag(string tag)
        {
            return View("Index", BlogRepository.GetAll().Where(p => p.Keywords.Contains(tag)));
        }

    }
}
