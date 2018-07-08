using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Models;

namespace Naif.Blog.ViewComponents
{
    public class UserProfileViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new UserProfile
            {
                Name = UserClaimsPrincipal.Identity.Name,
                EmailAddress = UserClaimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                ProfileImage = UserClaimsPrincipal.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
            };
            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return View(model);
        }
    }
}