using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Services;
using System.Linq;

namespace Naif.Blog.Controllers
{
	public class BlogController : BaseController
    {
        public BlogController(IBlogRepository blogRepository, IApplicationContext appContext) 
            : base(blogRepository, appContext) { }

        public IActionResult Index()
        {
            Blog.Posts = BlogRepository.GetAll(Blog.Id);
            return View("Index", Blog);
        }

        public IActionResult ViewCategory(string category)
        {
            Blog.Posts = BlogRepository.GetAll(Blog.Id).Where(p => p.Categories.Contains(category));
            return View("Index", Blog);
        }

        public IActionResult ViewPost(string slug)
        {
            Blog.Post = BlogRepository.GetAll(Blog.Id).Single(p => p.Slug == slug);
            return View(Blog);
        }

        public IActionResult ViewTag(string tag)
        {
            Blog.Posts = BlogRepository.GetAll(Blog.Id).Where(p => p.Keywords.Contains(tag));
            return View("Index", Blog);
        }
    }
}
