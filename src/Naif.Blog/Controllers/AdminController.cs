using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Services;

namespace Naif.Blog.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : BaseController
    {
        public AdminController(IBlogRepository blogRepository, IApplicationContext appContext) 
            : base(blogRepository, appContext) { }
        
        // GET
        public IActionResult Index()
        {
            // ReSharper disable once Mvc.ViewNotResolved
            return View("Index", Blog);
        }
    }
}