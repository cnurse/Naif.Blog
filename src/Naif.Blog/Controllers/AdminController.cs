using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Naif.Blog.Framework;
using Naif.Blog.Services;
using Naif.Blog.ViewModels;

namespace Naif.Blog.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class AdminController : BaseController
    {
        public AdminController(IBlogRepository blogRepository,
            IApplicationContext appContext)
            : base(blogRepository, appContext)
        {
        }
        
        public IActionResult Index()
        {
            var blogViewModel = new BlogViewModel
            {
                Blog = Blog,
                Themes = BlogRepository.GetThemes().Select(t => new SelectListItem { Value = t, Text = t }).ToList()
            };
            
            // ReSharper disable once Mvc.ViewNotResolved
            return View("Index", blogViewModel);
        }

        public IActionResult Save(Naif.Blog.Models.Blog blog)
        {
            var blogs = BlogRepository.GetBlogs();
            
            var match = blogs.SingleOrDefault(b => b.Id == blog.Id);

            if (match != null)
            {
                match.ByLine = blog.ByLine;
                match.Disclaimer = blog.Disclaimer;
                match.LocalUrl = blog.LocalUrl;
                match.Theme = blog.Theme;
                match.Title = blog.Title;
                match.Url = blog.Url;
            }
            BlogRepository.SaveBlogs(blogs);
            
            return Index();
        }
    }
}