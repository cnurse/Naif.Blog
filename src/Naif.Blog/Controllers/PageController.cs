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
        private readonly IAuthorizationService _authorizationService;
        
        public PageController(IAuthorizationService authorizationService,
            IBlogRepository blogRepository, 
            IApplicationContext appContext,
            IPageRepository pageRepository) 
            : base(blogRepository, appContext)
        {
            _authorizationService = authorizationService;
            _pageRepository = pageRepository;
        }
        
        public IActionResult Cancel(string returnUrl)
        {
            return Redirect(returnUrl);
        }
        
        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Clear()
        {
            return DisplayListView(0, string.Empty, "List");
        }
        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult CreatePage([FromForm] Page page, string returnUrl)
        {
            page.BlogId = Blog.Id;
            
            if (string.IsNullOrEmpty(page.Slug))
            {
                page.Slug = CreateSlug(page.Title);
            }

            _pageRepository.SavePage(page);

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

            ViewBag.IsNew = false;
            ViewBag.ReturnUrl = returnUrl;

            return View("EditPage", blogViewModel);
        }
        
        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult List(string filter, int? page)
        {
            return DisplayListView(page, filter, "List");
        }

        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult NewPage(string returnUrl)
        {
            var page = new Page
            {
                BlogId = Blog.Id
            };

            var blogViewModel = new BlogViewModel
            {
                Blog = Blog,
                Categories = BlogRepository.GetCategories(Blog.Id).Select(c => new SelectListItem { Value = c.Key, Text = c.Key }).ToList(),
                Pages = _pageRepository.GetAllPages(Blog.Id),
                Page = page
            };

            ViewBag.IsNew = true;
            ViewBag.ReturnUrl = returnUrl;

            return View("EditPage", blogViewModel);
            
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

        public IActionResult ViewPage(string slug)
        {
            var page = _pageRepository.GetAllPages(Blog.Id).SingleOrDefault(p => p.Slug == slug);

            if (page == null)
            {
                return new NotFoundResult();
            }
            
            var blogViewModel = new BlogViewModel
            {
                Blog = Blog,
                PageIndex = 0,
                Page = page
            };

            // ReSharper disable once Mvc.ViewNotResolved
            return View("ViewPage", blogViewModel);
        }

        private IActionResult DisplayListView(int? page, string filter, string view)
        {
            bool includeUnpublished = _authorizationService.AuthorizeAsync(User, "RequireAdminRole").Result.Succeeded;

            var blogViewModel = new BlogViewModel
            {
                Blog = Blog,
                Filter = filter,
                PageIndex = page ?? 0,
                Pages = _pageRepository.GetAllPages(Blog.Id).Where(p => (string.IsNullOrEmpty(filter) || p.Title?.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0) && (p.IsPublished || includeUnpublished))
            };
            
            ViewBag.PageIndex = page ?? 0;
            
            // ReSharper disable once Mvc.ViewNotResolved
            return View(view, blogViewModel);           
        }
    }
}