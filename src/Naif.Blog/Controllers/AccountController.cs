using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Services;

namespace Naif.Blog.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IBlogRepository blogRepository, IApplicationContext appContext) 
            : base(blogRepository, appContext) { }
        
        public async Task Login(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties() { RedirectUri = returnUrl });
        }
        
        public IActionResult AccessDenied()
        {
            return View("AccessDenied", Blog);
        }

        [Authorize]
        public IActionResult Profile()
        {
            return View("Profile", Blog);
        }

        [Authorize]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Auth0", new AuthenticationProperties
            {
                // Indicate here where Auth0 should redirect the user after a logout.
                // Note that the resulting absolute Uri must be whitelisted in the 
                // **Allowed Logout URLs** settings for the app.
                RedirectUri = Url.Action("Index", "Post")
            });
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}