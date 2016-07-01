using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Services;
using System.Threading.Tasks;

namespace Naif.Blog.ViewComponents
{
    public class TagCloudViewComponent : ViewComponent
    {
        private IBlogRepository _blogRepository;

        public TagCloudViewComponent(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(_blogRepository.GetTags());
        }

    }
}
