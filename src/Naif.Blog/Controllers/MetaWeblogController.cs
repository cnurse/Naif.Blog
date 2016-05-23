using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Naif.Blog.Models;
using Naif.Blog.Services;
using Naif.Blog.XmlRpc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Naif.Blog.Controllers
{
    [XmlRpcService]
    public class MetaWeblogController : BaseController
    {
        private string _rootPath;

        public MetaWeblogController(IHostingEnvironment env, IBlogRepository blogRepository) :base(blogRepository)
        {
            _rootPath = env.WebRootPath;
        }

        public IActionResult DeletePost(string key, string postid, string username, string password, bool publish)
        {
            Post post = BlogRepository.GetAll().FirstOrDefault(p => p.ID == postid);

            if (post != null)
            {
                BlogRepository.Delete(post);
            }

            return new XmlRpcResult(post != null);
        }

        public IActionResult EditPost(string postid, string username, string password, Post post, bool publish)
        {
            Post match = BlogRepository.GetAll().FirstOrDefault(p => p.ID == postid);

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

                BlogRepository.Save(match);
            }

            return new XmlRpcResult(match != null);
        }

        public IActionResult GetCategories(string blogid, string username, string password)
        {
            var categories = BlogRepository.GetCategories();

            var list = new List<object>();

            foreach (string category in categories.Keys)
            {
                list.Add(new { title = category });
            }

            return new XmlRpcResult(list.ToArray());
        }

        public IActionResult GetPost(string postid, string username, string password)
        {
            var post = BlogRepository.GetAll().FirstOrDefault(p => p.ID == postid);

            var info = new
                    {
                        description = post.Content,
                        title = post.Title,
                        dateCreated = post.PubDate,
                        wp_slug = post.Slug,
                        categories = post.Categories.ToArray(),
                        mt_keywords = post.Keywords,
                        postid = post.ID,
                        mt_excerpt = post.Excerpt
                    };

            return new XmlRpcResult(info);
        }

        public IActionResult GetRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            List<object> list = new List<object>();

            foreach (var post in BlogRepository.GetAll().Take(numberOfPosts))
            {
                var info = new
                            {
                                description = post.Content,
                                title = post.Title,
                                dateCreated = post.PubDate,
                                wp_slug = post.Slug,
                                postid = post.ID
                            };

                list.Add(info);
            }

            return new XmlRpcResult(list.ToArray());
        }

        public IActionResult GetUsersBlogs(string key, string username, string password)
        {
            var blogs =  new[]
                            {
                                new
                                {
                                    blogid = "1",
                                    blogName = "Naif.Blog",
                                    url = "http://localhost:56288/"
                                }
                            };
            return new XmlRpcResult(blogs);
        }

        public IActionResult NewMediaObject(string blogid, string username, string password, MediaObject media)
        {
            string extension = Path.GetExtension(media.Name);

            string relative = "/posts/files/" + Guid.NewGuid();

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".bin";
            }
            else
            {
                extension = "." + extension.Trim('.');
            }

            relative += extension;

            string file = _rootPath + relative.Replace("/", "\\");

            System.IO.File.WriteAllBytes(file, media.Bits);

            return new XmlRpcResult(new { url = $"{Request.Scheme}://{Request.Host}{relative}" });
        }

        public IActionResult NewPost(string blogid, string username, string password, Post post, bool publish)
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
            BlogRepository.Save(post);

            return new XmlRpcResult(post.ID);
        }
    }
}
