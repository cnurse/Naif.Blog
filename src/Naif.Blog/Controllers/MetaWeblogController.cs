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

        public MetaWeblogController(IBlogRepository blogRepository, IApplicationContext appContext, IOptions<XmlRpcSecurityOptions> optionsAccessor) 
            :base(blogRepository, appContext)
        {
            _securityOptions = optionsAccessor.Value;
        }

        public IActionResult GetUsersBlogs(string key, string username, string password)
        {
            return CheckSecurity(username, password, () =>
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
        
        

        public IActionResult DeletePost(string key, string postid, string username, string password, bool publish)
        {
            return CheckSecurity(username, password, () =>
            {
                Post post = BlogRepository.GetAllPosts(Blog.Id).FirstOrDefault(p => p.PostId == postid);

                if (post != null)
                {
                    post.BlogId = Blog.Id;
                    BlogRepository.DeletePost(post);
                }

                return new XmlRpcResult(post != null);
            });
        }

        public IActionResult EditPost(string postid, string username, string password, Post post, bool publish)
        {
            return CheckSecurity(username, password, () =>
            {
                Post match = BlogRepository.GetAllPosts(Blog.Id).FirstOrDefault(p => p.PostId == postid);

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

                    BlogRepository.SavePost(match);
                }

                return new XmlRpcResult(match != null);
            });
        }

        public IActionResult GetPost(string postid, string username, string password)
        {
            return CheckSecurity(username, password, () =>
            {
                var post = BlogRepository.GetAllPosts(Blog.Id).FirstOrDefault(p => p.PostId == postid);

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

        public IActionResult GetRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            return CheckSecurity(username, password, () =>
            {
                List<object> list = new List<object>();

                foreach (var post in BlogRepository.GetAllPosts(blogid).Take(numberOfPosts))
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

        public IActionResult NewPost(string blogid, string username, string password, Post post, bool publish)
        {
            return CheckSecurity(username, password, () =>
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
                post.BlogId = blogid;
                BlogRepository.SavePost(post);

                return new XmlRpcResult(post.PostId);
            });
        }
        
        
        
        public IActionResult EditPage(string pageid, string username, string password, Page page, bool publish)
        {
            return CheckSecurity(username, password, () =>
            {
                Page match = BlogRepository.GetAllPages(Blog.Id).FirstOrDefault(p => p.PageId == pageid);

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

                    BlogRepository.SavePage(match);
                }

                return new XmlRpcResult(match != null);
            });
        }

        public IActionResult GetPage(string blogid, string pageid, string username, string password)
        {
            return CheckSecurity(username, password, () =>
            {
                var page = BlogRepository.GetAllPages(Blog.Id).FirstOrDefault(p => p.PageId == pageid);

                var info = new
                {
                    description = page.Content,
                    title = page.Title,
                    dateCreated = page.PubDate,
                    wp_slug = page.Slug,
                    pageid = page.PageId,
                    wp_page_parent_id = page.ParentPageId
                };

                return new XmlRpcResult(info);
            });
        }

        public IActionResult GetPageList(string blogid, string username, string password)
        {
            return CheckSecurity(username, password, () =>
            {
                List<object> list = new List<object>();

                foreach (var page in BlogRepository.GetAllPages(blogid))
                {
                    var info = new
                    {
                        page_title = page.Title,
                        dateCreated = page.PubDate,
                        page_id = page.PageId,
                        page_parent_id = page.ParentPageId
                    };

                    list.Add(info);
                }

                return new XmlRpcResult(list.ToArray());
            });
        }

        public IActionResult GetPages(string blogid, string username, string password, int numberOfPosts)
        {
            return CheckSecurity(username, password, () =>
            {
                List<object> list = new List<object>();

                foreach (var page in BlogRepository.GetAllPages(blogid).Take(numberOfPosts))
                {
                    var info = new
                    {
                        description = page.Content,
                        title = page.Title,
                        dateCreated = page.PubDate,
                        wp_slug = page.Slug,
                        page_id = page.PageId,
                        page_parent_id = page.ParentPageId
                    };

                    list.Add(info);
                }

                return new XmlRpcResult(list.ToArray());
            });
        }

        public IActionResult NewPage(string blogid, string username, string password, Page page, bool publish)
        {
            return CheckSecurity(username, password, () =>
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
                page.BlogId = blogid;
                BlogRepository.SavePage(page);

                return new XmlRpcResult(page.PageId);
            });
        }

        
        
        
        public IActionResult GetCategories(string blogid, string username, string password)
        {
            return CheckSecurity(username, password, () =>
            {

                var categories = BlogRepository.GetCategories(blogid);

                var list = new List<object>();

                foreach (string category in categories.Keys)
                {
                    list.Add(new {title = category});
                }

                return new XmlRpcResult(list.ToArray());
            });
        }

        public IActionResult NewMediaObject(string blogid, string username, string password, MediaObject media)
        {
            return CheckSecurity(username, password, () =>
            {

                string relative = BlogRepository.SaveMedia(blogid, media);

                return new XmlRpcResult(new {url = $"{Request.Scheme}://{Request.Host}{relative}"});
            });
        }

        public IActionResult CheckSecurity(string username, string password, Func<IActionResult> secureFunc)
        {
            if (_securityOptions.Username == username && _securityOptions.Password == password)
            {
                return secureFunc();
            }

            return new UnauthorizedResult();
        }
    }
}
