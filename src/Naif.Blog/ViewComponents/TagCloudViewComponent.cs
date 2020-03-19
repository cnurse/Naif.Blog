using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Services;
using System.Threading.Tasks;

namespace Naif.Blog.ViewComponents
{
	public class TagCloudViewComponent : BaseViewComponent
	{
		public TagCloudViewComponent(IBlogRepository blogRepository, IApplicationContext appContext)
			: base(blogRepository, appContext)
		{
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			Dictionary<string, int> model = null;
			
			await Task.Run(() =>
			{
				model = BlogRepository.GetTags(Blog.Id);
			});
			
			// ReSharper disable once Mvc.ViewComponentViewNotResolved
			return View(model);
		}
	}
}