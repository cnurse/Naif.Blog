using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Services;

namespace Naif.Blog.Controllers
{
    public class ErrorController : BaseController
    {
        public ErrorController(IBlogRepository blogRepository, IApplicationContext appContext) 
            : base(blogRepository, appContext) { }


        public IActionResult Index()
        {
            return Code(Response.StatusCode.ToString());
        }
        
        [Route("/error/code/{errCode}")]
        public IActionResult Code(string errCode)
        {
            if (errCode == "404")
            {
                return View("404", Blog);
            }

            if (errCode == "500")
            {
                return View("500", Blog);
            }

            return View("Unknown", Blog);
        }
    }
}