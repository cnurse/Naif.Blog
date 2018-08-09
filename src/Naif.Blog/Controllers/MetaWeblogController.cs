using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Models;
using Naif.Blog.Services;
using Naif.Blog.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Naif.Blog.Security;

namespace Naif.Blog.Controllers
{
    [XmlRpcService]
    public class MetaWeblogController : BaseController
    {
        private readonly XmlRpcSecurityOptions _securityOptions;
        private readonly IPageRepository _pageRepository;
        private readonly IPostRepository _postRepository;

        public MetaWeblogController(IBlogRepository blogRepository, 
            IApplicationContext appContext, 
            IPageRepository pageRepository, 
            IPostRepository postRepository, 
            IOptions<XmlRpcSecurityOptions> optionsAccessor) 
            :base(blogRepository, appContext)
        {
            _securityOptions = optionsAccessor.Value;
            _pageRepository = pageRepository;
            _postRepository = postRepository;
        }

        public IActionResult GetUsersBlogs(string key, string userName, string password)
        {
            return CheckSecurity(userName, password, () =>
            {

                var blogs = new[]
                {
                    new
                    {
                        blogid = Blog.Id,
                        blogName = Blog.Title,
                        url = Blog.Url
                    }
                };
                return new XmlRpcResult(blogs);
            });
        }
        
        public IActionResult DeletePost(string key, string postId, string userName, string password, bool publish)
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

        public IActionResult EditPost(string postId, string userName, string password, Post post, bool publish)
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

        public IActionResult GetPost(string postId, string userName, string password)
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

        public IActionResult GetRecentPosts(string blogId, string userName, string password, int numberOfPosts)
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

        public IActionResult NewPost(string blogId, string userName, string password, Post post, bool publish)
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
        
        public IActionResult DeletePage(string blogId,  string userName, string password, string pageId)
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
        
        public IActionResult EditPage(string blogId, string pageId, string userName, string password, Page page, bool publish)
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

        public IActionResult GetPage(string blogId, string pageId, string userName, string password)
        {
            return CheckSecurity(userName, password, () =>
            {
                var page = _pageRepository.GetAllPages(Blog.Id).FirstOrDefault(p => p.PageId == pageId);

                return new XmlRpcResult(CreatePage(page));
            });
        }

        public IActionResult GetPageList(string blogId, string userName, string password)
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

        public IActionResult GetPages(string blogId, string userName, string password, int numberOfPosts)
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

        public IActionResult NewPage(string blogId, string userName, string password, Page page, bool publish)
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

        
        public IActionResult GetCategories(string blogId, string userName, string password)
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

        public IActionResult NewMediaObject(string blogId, string userName, string password, MediaObject media)
        {
            return CheckSecurity(userName, password, () =>
            {

                string relative = BlogRepository.SaveMedia(blogId, media);

                return new XmlRpcResult(new {url = $"{Request.Scheme}://{Request.Host}{relative}"});
            });
        }

        public IActionResult CheckSecurity(string userName, string password, Func<IActionResult> secureFunc)
        {
            if (_securityOptions.Username == userName && _securityOptions.Password == password)
            {
                return secureFunc();
            }

            return new UnauthorizedResult();
        }
    }
}
