using Microsoft.AspNet.Mvc;
using Naif.Blog.Services;
using Naif.Blog.XmlRpc;
using System.Collections.Generic;
using System.Linq;

namespace Naif.Blog.Controllers
{
    [XmlRpcService]
    public class MetaWeblogController : Controller
    {
        private IBlogRepository _blogRepository;

        public MetaWeblogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public IActionResult GetPost(string postid, string username, string password)
        {
            var post = _blogRepository.GetAll().FirstOrDefault(p => p.ID == postid);

            var info = new
                    {
                        description = post.Content,
                        title = post.Title,
                        dateCreated = post.PubDate,
                        wp_slug = post.Slug,
                        categories = new string[0], //post.Categories.ToArray(),
                        mt_keywords = new string[0], //post.Tags.ToArray(),
                        postid = post.ID,
                        mt_excerpt = post.Excerpt
                    };

            return new XmlRpcResult(info);
        }

        public IActionResult GetRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            List<object> list = new List<object>();

            foreach (var post in _blogRepository.GetAll().Take(numberOfPosts))
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
    }
}
