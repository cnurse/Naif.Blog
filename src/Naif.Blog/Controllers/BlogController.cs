using System;
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

        public IActionResult Index(int? page)
        {
            Blog.Posts = BlogRepository.GetAll(Blog.Id);

            ViewData["ActionName"] = "Index";
            ViewData["Parameter"] = String.Empty;
            ViewData["Value"] = String.Empty;
            ViewData["Page"] = page ?? 0;

            return View("Index", Blog);
        }

        public IActionResult ViewCategory(string category, int? page)
        {
            Blog.Posts = BlogRepository.GetAll(Blog.Id).Where(p => p.Categories.Contains(category));

            ViewData["ActionName"] = "ViewCategory";
            ViewData["Parameter"] = "category";
            ViewData["Value"] = category;
            ViewData["Page"] = page ?? 0;

            return View("Index", Blog);
        }

        public IActionResult ViewPost(string slug)
        {
            Blog.Post = BlogRepository.GetAll(Blog.Id).Single(p => p.Slug == slug);
            return View(Blog);
        }

        public IActionResult ViewTag(string tag, int? page)
        {
            Blog.Posts = BlogRepository.GetAll(Blog.Id).Where(p => p.Keywords.Contains(tag));

            ViewData["ActionName"] = "ViewTag";
            ViewData["Parameter"] = "tag";
            ViewData["Value"] = tag;
            ViewData["Page"] = page ?? 0;

            return View("Index", Blog);
        }
    }
}
