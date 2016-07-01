using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Naif.Blog.Models;
using System.Threading.Tasks;

namespace Naif.Blog.ViewComponents
{
    public class DisclaimerViewComponent : ViewComponent
    {
        private BlogOptions _options;

        public DisclaimerViewComponent(IOptions<BlogOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(_options);
        }
    }
}
