using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Models;
using Naif.Blog.Services;
using Naif.Blog.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Naif.Blog.Security;

namespace Naif.Blog.Controllers
{
    [Route("MetaWeblog")]
    public class MetaWeblogController : BaseController
    {
        private readonly IWebHostEnvironment _environment;
        private readonly XmlRpcSecurityOptions _securityOptions;
        private readonly IPageRepository _pageRepository;
        private readonly IPostRepository _postRepository;

        public MetaWeblogController(IWebHostEnvironment environment,
            IBlogRepository blogRepository, 
            IApplicationContext appContext, 
            IPageRepository pageRepository, 
            IPostRepository postRepository, 
            IOptions<XmlRpcSecurityOptions> optionsAccessor) 
            :base(blogRepository, appContext)
        {
            _environment = environment;
            _securityOptions = optionsAccessor.Value;
            _pageRepository = pageRepository;
            _postRepository = postRepository;
        }

        public IActionResult Index()
        {
            var methodName = ControllerContext.HttpContext.Items["Xml-Rpc-MethodName"] as string;
            var methodParams = ControllerContext.HttpContext.Items["Xml-Rpc-Parameters"] as List<object>;

            IActionResult result;
            
            switch (methodName)
            {
                case "getUsersBlogs":
                    result = GetUsersBlogs(methodParams[0].ToString(), methodParams[1].ToString(), methodParams[2].ToString());
                    break;
                case "getCategories":
                    result = GetCategories(methodParams[0].ToString(), methodParams[1].ToString(), methodParams[2].ToString());
                    break;
                case "getRecentPosts":
                    result = GetRecentPosts(methodParams[0].ToString(), methodParams[1].ToString(), methodParams[2].ToString(), (int)methodParams[3]);
                    break;
                case "getPost":
                    result = GetPost(methodParams[0].ToString(), methodParams[1].ToString(), methodParams[2].ToString());
                    break;
                case "deletePost":
                    result = DeletePost(methodParams[0].ToString(), methodParams[1].ToString(), methodParams[2].ToString(), methodParams[3].ToString());
                    break;
                case "editPost":
                    result = EditPost(methodParams[0].ToString(), methodParams[1].ToString(), methodParams[2].ToString(), (Post)methodParams[3], (bool)methodParams[4]);
                    break;
                case "newPost":
                    result = NewPost(methodParams[0].ToString(), methodParams[1].ToString(), methodParams[2].ToString(), (Post)methodParams[3], (bool)methodParams[4]);
                    break;
                case "newMediaObject":
                    result = NewMediaObject(methodParams[0].ToString(), methodParams[1].ToString(), methodParams[2].ToString(), (MediaObject)methodParams[3]);
                    break;
                default:
                    result = new XmlRpcResult(null);
                    break;
            }

            return result;
        }

        private IActionResult GetUsersBlogs(string key, string userName, string password)
        {
            return CheckSecurity(userName, password, () =>
            {

                var blogUrl = (_environment.IsDevelopment()) ? Blog.LocalUrl : Blog.Url;
                var blogs = new[]
                {
                    new
                    {
                        blogid = Blog.Id,
                        blogName = Blog.Title,
                        url = $"http://{blogUrl}"  
                    }
                };
                return new XmlRpcResult(blogs);
            });
        }

        private IActionResult DeletePost(string key, string postId, string userName, string password)
        {
            return CheckSecurity(userName, password, () =>
            {
                Post post = _postRepository.GetAllPosts(Blog.Id).FirstOrDefault(p => p.PostId == postId);

                if (post != null)
                {
                    post.BlogId = Blog.Id;
                    _postRepository.DeletePost(post);
                }

                return new XmlRpcResult(post != null);
            });
        }

        private IActionResult EditPost(string postId, string userName, string password, Post post, bool publish)
        {
            return CheckSecurity(userName, password, () =>
            {
                Post match = _postRepository.GetAllPosts(Blog.Id).FirstOrDefault(p => p.PostId == postId);

                if (match != null)
                {
                    match.Title = post.Title;
                    match.Excerpt = post.Excerpt;
                    match.Content = post.Content;

                    if (!string.Equals(match.Slug, post.Slug, StringComparison.OrdinalIgnoreCase))
                    {
                        match.Slug = CreateSlug(post.Slug);
                    }

                    match.Categories = post.Categories;
                    match.Keywords = post.Keywords;
                    match.IsPublished = publish;

                    _postRepository.SavePost(match);
                }

                return new XmlRpcResult(match != null);
            });
        }

        private IActionResult GetPost(string postId, string userName, string password)
        {
            return CheckSecurity(userName, password, () =>
            {
                var post = _postRepository.GetAllPosts(Blog.Id).FirstOrDefault(p => p.PostId == postId);

                var info = new
                {
                    description = post.Content,
                    title = post.Title,
                    dateCreated = post.PubDate,
                    wp_slug = post.Slug,
                    categories = post.Categories.ToArray(),
                    mt_keywords = post.Keywords,
                    postid = post.PostId,
                    mt_excerpt = post.Excerpt
                };

                return new XmlRpcResult(info);
            });
        }

        private IActionResult GetRecentPosts(string blogId, string userName, string password, int numberOfPosts)
        {
            return CheckSecurity(userName, password, () =>
            {
                List<object> list = new List<object>();

                foreach (var post in _postRepository.GetAllPosts(blogId).Take(numberOfPosts))
                {
                    var info = new
                    {
                        description = post.Content,
                        title = post.Title,
                        dateCreated = post.PubDate,
                        wp_slug = post.Slug,
                        postid = post.PostId
                    };

                    list.Add(info);
                }

                return new XmlRpcResult(list.ToArray());
            });
        }

        private IActionResult NewPost(string blogId, string userName, string password, Post post, bool publish)
        {
            return CheckSecurity(userName, password, () =>
            {

                try
                {
                    if (!string.IsNullOrWhiteSpace(post.Slug))
                    {
                        post.Slug = CreateSlug(post.Slug);
                    }
                    else
                    {
                        post.Slug = CreateSlug(post.Title);
                    }
                }
                catch (Exception exc)
                {
                    return new XmlRpcResult(exc);
                }

                post.IsPublished = publish;
                post.BlogId = blogId;
                _postRepository.SavePost(post);

                return new XmlRpcResult(post.PostId);
            });
        }
        
        private object CreatePage(Page page)
        {
            var info = new
            {
                description = page.Content,
                title = page.Title,
                dateCreated = page.PubDate,
                wp_slug = page.Slug,
                page_id = page.PageId,
                wp_page_parent_id = page.ParentPageId
            };

            return info;
        }

        private IActionResult DeletePage(string blogId,  string userName, string password, string pageId)
        {
            return CheckSecurity(userName, password, () =>
            {
                Page page = _pageRepository.GetAllPages(Blog.Id).FirstOrDefault(p => p.PageId == pageId);

                if (page != null)
                {
                    page.BlogId = Blog.Id;
                    _pageRepository.DeletePage(page);
                }

                return new XmlRpcResult(page != null);
            });
        }

        private IActionResult EditPage(string blogId, string pageId, string userName, string password, Page page, bool publish)
        {
            return CheckSecurity(userName, password, () =>
            {
                Page match = _pageRepository.GetAllPages(Blog.Id).FirstOrDefault(p => p.PageId == pageId);

                if (match != null)
                {
                    match.Title = page.Title;
                    match.Content = page.Content;

                    if (!string.Equals(match.Slug, page.Slug, StringComparison.OrdinalIgnoreCase))
                    {
                        match.Slug = CreateSlug(page.Slug);
                    }

                    match.Keywords = page.Keywords;
                    match.IsPublished = publish;

                    _pageRepository.SavePage(match);
                }

                return new XmlRpcResult(match != null);
            });
        }

        private IActionResult GetPage(string blogId, string pageId, string userName, string password)
        {
            return CheckSecurity(userName, password, () =>
            {
                var page = _pageRepository.GetAllPages(Blog.Id).FirstOrDefault(p => p.PageId == pageId);

                return new XmlRpcResult(CreatePage(page));
            });
        }

        private IActionResult GetPageList(string blogId, string userName, string password)
        {
            return CheckSecurity(userName, password, () =>
            {
                List<object> list = new List<object>();

                foreach (var page in _pageRepository.GetAllPages(blogId))
                {
                    var info = new
                    {
                        page_title = page.Title,
                        dateCreated = page.PubDate,
                        page_id = page.PageId,
                        wp_page_parent_id = page.ParentPageId
                    };

                    list.Add(info);
                }

                return new XmlRpcResult(list.ToArray());
            });
        }

        private IActionResult GetPages(string blogId, string userName, string password, int numberOfPosts)
        {
            return CheckSecurity(userName, password, () =>
            {
                List<object> list = new List<object>();

                foreach (var page in _pageRepository.GetAllPages(blogId).Take(numberOfPosts))
                {
                    list.Add(CreatePage(page));
                }

                return new XmlRpcResult(list.ToArray());
            });
        }

        private IActionResult NewPage(string blogId, string userName, string password, Page page, bool publish)
        {
            return CheckSecurity(userName, password, () =>
            {

                try
                {
                    if (!string.IsNullOrWhiteSpace(page.Slug))
                    {
                        page.Slug = CreateSlug(page.Slug);
                    }
                    else
                    {
                        page.Slug = CreateSlug(page.Title);
                    }
                }
                catch (Exception exc)
                {
                    return new XmlRpcResult(exc);
                }

                page.IsPublished = publish;
                page.BlogId = blogId;
                _pageRepository.SavePage(page);

                return new XmlRpcResult(page.PageId);
            });
        }


        private IActionResult GetCategories(string blogId, string userName, string password)
        {
            return CheckSecurity(userName, password, () =>
            {

                var categories = BlogRepository.GetCategories(blogId);

                var list = new List<object>();

                foreach (string category in categories.Keys)
                {
                    list.Add(new {title = category});
                }

                return new XmlRpcResult(list.ToArray());
            });
        }

        private IActionResult NewMediaObject(string blogId, string userName, string password, MediaObject media)
        {
            return CheckSecurity(userName, password, () =>
            {

                string relative = BlogRepository.SaveMedia(blogId, media);

                return new XmlRpcResult(new {url = $"{Request.Scheme}://{Request.Host}{relative}"});
            });
        }

        private IActionResult CheckSecurity(string userName, string password, Func<IActionResult> secureFunc)
        {
            if (_securityOptions.Username == userName && _securityOptions.Password == password)
            {
                return secureFunc();
            }

            return new UnauthorizedResult();
        }
    }
}
