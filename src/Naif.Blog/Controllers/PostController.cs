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
	[Route("Post")]
    public class PostController : BaseController
    {
        private readonly IPostRepository _postRepository;
        private readonly IAuthorizationService _authorizationService;

        public PostController(IAuthorizationService authorizationService,
            IBlogRepository blogRepository,
            IApplicationContext appContext,
            IPostRepository postRepository)
            : base(blogRepository, appContext)
        {
            _authorizationService = authorizationService;
            _postRepository = postRepository;
        }

        [HttpGet]
        [Route("Cancel/{returnUrl}")]
        public IActionResult Cancel(string returnUrl)
        {
            return Redirect(returnUrl);
        }
        
        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        [Route("Clear")]
        public IActionResult Clear()
        {
            return DisplayListView(0, string.Empty, "List");
        }

        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        [Route("EditPost/{id}/{returnUrl}")]
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
        
        [HttpGet]
        [Route("")]
        [Route("Index/{page?}")]
        [Route("/{page?}")]
        public IActionResult Index(int? page)
        {
            return DisplayListView(page, string.Empty, "Index");
        }

        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        [Route("{filter}/{page?}")]
        public IActionResult List(string filter, int? page)
        {
            return DisplayListView(page, filter, "List");
        }

        [Authorize(Policy = "RequireAdminRole")]
        [Route("NewPost/{returnUrl}")]
        public IActionResult NewPost(string returnUrl)
        {
            var post = new Post
            {
                BlogId = Blog.Id
            };

            var blogViewModel = new BlogViewModel
            {
                Blog = Blog,
                Categories = BlogRepository.GetCategories(Blog.Id).Select(c => new SelectListItem { Value = c.Key, Text = c.Key }).ToList(),
                Post = post
            };

            ViewBag.ReturnUrl = returnUrl;

            return View("EditPost", blogViewModel);
            
        }
        
        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        [Route("SavePost/{returnUrl}")]
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

        [HttpGet]
        [Route("~/category/{category}/{page?}")]
        public IActionResult ViewCategory(string category, int? page)
        {
            var blogViewModel = new BlogViewModel
            {
                Blog = Blog,
                PageIndex = page ?? 0,
                Posts = _postRepository.GetAllPosts(Blog.Id).Where(p => p.Categories.Contains(category) && p.IsPublished)
            };

            // ReSharper disable once Mvc.ViewNotResolved
            return View("Index", blogViewModel);
        }

        [HttpGet]
        [Route("{slug}")]
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

        [HttpGet]
        [Route("~/tag/{tag}/{page?}")]
        public IActionResult ViewTag(string tag, int? page)
        {
            var blogViewModel = new BlogViewModel
            {
                Blog = Blog,
                PageIndex = page ?? 0,
                Posts = _postRepository.GetAllPosts(Blog.Id).Where(p => p.Keywords.Contains(tag) && p.IsPublished)
            };

            // ReSharper disable once Mvc.ViewNotResolved
            return View("Index", blogViewModel);
        }
        
        private IActionResult DisplayListView(int? page, string filter, string view)
        {
            bool includeUnpublished = _authorizationService.AuthorizeAsync(User, "RequireAdminRole").Result.Succeeded;

            var blogViewModel = new BlogViewModel
            {
                Blog = Blog,
                Filter = filter,
                PageIndex = page ?? 0,
                Posts = _postRepository.GetAllPosts(Blog.Id).Where(p => (string.IsNullOrEmpty(filter) || p.Title?.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0) && (p.IsPublished || includeUnpublished))
            };
            
            ViewBag.PageIndex = page ?? 0;
            
            // ReSharper disable once Mvc.ViewNotResolved
            return View(view, blogViewModel);           
        }
    }
}
