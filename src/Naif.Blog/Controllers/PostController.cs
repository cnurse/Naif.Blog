using System;
using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Services;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Naif.Blog.Models;
using Naif.Blog.ViewModels;

namespace Naif.Blog.Controllers
{
	public class PostController : BaseController
    {
        private readonly IPostRepository _postRepository;

        public PostController(IBlogRepository blogRepository,
            IApplicationContext appContext,
            IPostRepository postRepository)
            : base(blogRepository, appContext)
        {
            _postRepository = postRepository;
        }

        public IActionResult Cancel(string returnUrl)
        {
            return Redirect(returnUrl);
        }
        
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult EditPost(string id, string returnUrl)
        {
            var post = _postRepository.GetAllPosts(Blog.Id).SingleOrDefault(p => p.PostId == id);

            if (post == null)
            {
                return new NotFoundResult();
            }
            
            var blogViewModel = new BlogViewModel
            {
                Blog = Blog,
                Categories = BlogRepository.GetCategories(Blog.Id).Select(c => new SelectListItem { Value = c.Key, Text = c.Key }).ToList(),
                Post = post
            };

            ViewBag.ReturnUrl = returnUrl;

            return View("EditPost", blogViewModel);
        }

        public IActionResult Index(int? page)
        {
            return DisplayListView(page, "Index");
        }

        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult List(int? page)
        {
            return DisplayListView(page, "List");
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult SavePost([FromForm] Post post, string returnUrl)
        {
            var match = _postRepository.GetAllPosts(Blog.Id).SingleOrDefault(p => p.PostId == post.PostId);

            if (match != null)
            {
                match.Title = post.Title;
                match.Excerpt = post.Excerpt;
                match.Content = post.Content;

                if (!string.Equals(match.Slug, post.Slug, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(match.Slug))
                {
                    match.Slug = CreateSlug(post.Title);
                }

                match.Categories = post.Categories;
                match.Keywords = post.Keywords;
                match.IsPublished = post.IsPublished;

                _postRepository.SavePost(match);
            }

            return Redirect(returnUrl);
        }

        public IActionResult ViewCategory(string category, int? page)
        {
            var blogViewModel = new BlogViewModel
            {
                Blog = Blog,
                PageIndex = page ?? 0,
                Posts = _postRepository.GetAllPosts(Blog.Id).Where(p => p.Categories.Contains(category))
            };

            // ReSharper disable once Mvc.ViewNotResolved
            return View("Index", blogViewModel);
        }

        public IActionResult ViewPost(string slug)
        {
            var post = _postRepository.GetAllPosts(Blog.Id).SingleOrDefault(p => p.Slug == slug);

            if (post == null)
            {
                return new NotFoundResult();
            }
            
            var blogViewModel = new BlogViewModel
            {
                Blog = Blog,
                PageIndex = 0,
                Post = post
            };

            // ReSharper disable once Mvc.ViewNotResolved
            return View("ViewPost", blogViewModel);
        }

        public IActionResult ViewTag(string tag, int? page)
        {
            var blogViewModel = new BlogViewModel
            {
                Blog = Blog,
                PageIndex = page ?? 0,
                Posts = _postRepository.GetAllPosts(Blog.Id).Where(p => p.Keywords.Contains(tag))
            };

            // ReSharper disable once Mvc.ViewNotResolved
            return View("Index", blogViewModel);
        }
        
        private IActionResult DisplayListView(int? page, string view)
        {
            var blogViewModel = new BlogViewModel
            {
                Blog = Blog,
                PageIndex = page ?? 0,
                Posts = _postRepository.GetAllPosts(Blog.Id)
            };
            
            ViewBag.PageIndex = page ?? 0;
            
            // ReSharper disable once Mvc.ViewNotResolved
            return View(view, blogViewModel);           
        }
    }
}
