using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Models;
using Naif.Blog.Services;

namespace Naif.Blog.ViewComponents
{
    public class NavigationViewComponent : BaseViewComponent
    {
        private readonly IPageRepository _pageRepository;

        public NavigationViewComponent(IBlogRepository blogRepository, IApplicationContext appContext, IPageRepository pageRepository)
            : base(blogRepository, appContext)
        {
            _pageRepository = pageRepository;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(string parent)
        {
            var menu = new Menu
            {
                CssClass = "navbar-nav",
                IsActiveCssClass = "active",
                Items = new List<MenuItem>()
            };

            await Task.Run(() =>
            {
                foreach(var page in _pageRepository.GetAllPages(Blog.Id).Where(p => p.ParentPageId == parent))
                {
                    if (page.PageType == PageType.Blog)
                    {
                        menu.Items.Add(new MenuItem
                        {
                            Controller = "Post",
                            Action = "Index",
                            IsActive = false,
                            Text = page.Title
                        });
                    }
                    else
                    {
                        menu.Items.Add(new MenuItem
                        {
                            IsActive = false,
                            Link = $"/page/{page.Slug}",
                            Text = page.Title
                        });
                    }
                }
            });

            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return View(menu);
        }
    }
}