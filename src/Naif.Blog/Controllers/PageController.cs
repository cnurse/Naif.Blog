using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Naif.Blog.Framework;
using Naif.Blog.Models;
using Naif.Blog.Services;
using Naif.Blog.ViewModels;

namespace Naif.Blog.Controllers
{
    public class PageController : BaseController
    {
        private readonly IPageRepository _pageRepository;

        public PageController(IBlogRepository blogRepository, 
            IApplicationContext appContext,
            IPageRepository pageRepository) 
            : base(blogRepository, appContext)
        {
            _pageRepository = pageRepository;
        }
        
        public IActionResult Cancel(string returnUrl)
        {
            return Redirect(returnUrl);
        }
        
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult EditPage(string id, string returnUrl)
        {
            var page = _pageRepository.GetAllPages(Blog.Id).SingleOrDefault(p => p.PageId == id);

            if (page == null)
            {
                return new NotFoundResult();
            }
            
            var blogViewModel = new BlogViewModel
            {
                Blog = Blog,
                Categories = BlogRepository.GetCategories(Blog.Id).Select(c => new SelectListItem { Value = c.Key, Text = c.Key }).ToList(),
                Pages = _pageRepository.GetAllPages(Blog.Id),
                Page = page
            };

            ViewBag.ReturnUrl = returnUrl;

            return View("EditPage", blogViewModel);
        }
        
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult List(int? page)
        {
            var blogViewModel = new BlogViewModel
            {
                Blog = Blog,
                PageIndex = page ?? 0,
                Pages = _pageRepository.GetAllPages(Blog.Id)
            };

            // ReSharper disable once Mvc.ViewNotResolved
            return View("List", blogViewModel);
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult SavePage([FromForm] Page page, string returnUrl)
        {
            var match = _pageRepository.GetAllPages(Blog.Id).SingleOrDefault(p => p.PageId == page.PageId);

            if (match != null)
            {
                match.Title = page.Title;
                match.Excerpt = page.Excerpt;
                match.Content = page.Content;

                if (!string.Equals(match.Slug, page.Slug, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(match.Slug))
                {
                    match.Slug = CreateSlug(page.Title);
                }

                match.Categories = page.Categories;
                match.Keywords = page.Keywords;
                match.IsPublished = page.IsPublished;

                _pageRepository.SavePage(match);
            }

            return Redirect(returnUrl);
        }
    }
}