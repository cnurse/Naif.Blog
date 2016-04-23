using System;
using System.Collections.Generic;
using Naif.Blog.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNet.Hosting;
using System.IO;
using System.Xml.Linq;

namespace Naif.Blog.Services
{
    public class XmlBlogRepository : IBlogRepository
    {
        private ILogger _logger;
        private IMemoryCache _memoryCache;
        private string _postsCacheKey = "posts";
        private string _postsFolder;

        public XmlBlogRepository(IHostingEnvironment env, 
                                    IMemoryCache memoryCache, 
                                    ILoggerFactory loggerFactory)
        {
            _memoryCache = memoryCache;
            _logger = loggerFactory.CreateLogger<XmlBlogRepository>();
            _postsFolder = env.WebRootPath + "/posts/";
        }

        public void Delete(Post post)
        {
            string file = Path.Combine(_postsFolder, post.ID + ".xml");
            File.Delete(file);
            _logger.LogInformation($"{_postsCacheKey} cleared.");
        }

        public IEnumerable<Post> GetAll()
        {
            IEnumerable<Post> posts;

            if (!_memoryCache.TryGetValue(_postsCacheKey, out posts))
            {
                // fetch the value from the source
                posts = GetPosts();

                // store in the cache
                _memoryCache.Set(_postsCacheKey, 
                    posts, 
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(2)));
                _logger.LogInformation($"{_postsCacheKey} updated from source.");
            }
            else
            {
                _logger.LogInformation($"{_postsCacheKey} retrieved from cache.");
            }

            return posts;
        }

        public void Save(Post post)
        {
            string file = Path.Combine(_postsFolder, post.ID + ".xml");
            post.LastModified = DateTime.UtcNow;

            XDocument doc = new XDocument(
                            new XElement("post",
                                new XElement("title", post.Title),
                                new XElement("slug", post.Slug),
                                new XElement("author", post.Author),
                                new XElement("pubDate", post.PubDate.ToString("yyyy-MM-dd HH:mm:ss")),
                                new XElement("lastModified", post.LastModified.ToString("yyyy-MM-dd HH:mm:ss")),
                                new XElement("excerpt", post.Excerpt),
                                new XElement("content", post.Content),
                                new XElement("ispublished", post.IsPublished)
                            ));

            _memoryCache.Remove(_postsCacheKey);

            if (!File.Exists(file)) // New post
            {
                _logger.LogInformation($"New Post - {post.ID} created.");
            }
            else
            {
                _logger.LogInformation($"Post - {post.ID} updated.");
            }

            _logger.LogInformation($"{_postsCacheKey} cleared.");

            //doc.Save(file);
        }

        private IEnumerable<Post> GetPosts()
        {
            if (!Directory.Exists(_postsFolder))
            {
                Directory.CreateDirectory(_postsFolder);
            }

            List<Post> list = new List<Post>();

            // Can this be done in parallel to speed it up?
            foreach (string file in Directory.EnumerateFiles(_postsFolder, "*.xml", SearchOption.TopDirectoryOnly))
            {
                XElement doc = XElement.Load(file);

                Post post = new Post()
                                {
                                    ID = Path.GetFileNameWithoutExtension(file),
                                    Title = ReadValue(doc, "title"),
                                    Author = ReadValue(doc, "author"),
                                    Excerpt = ReadValue(doc, "excerpt"),
                                    Content = ReadValue(doc, "content"),
                                    Slug = ReadValue(doc, "slug").ToLowerInvariant(),
                                    PubDate = DateTime.Parse(ReadValue(doc, "pubDate")),
                                    LastModified = DateTime.Parse(ReadValue(doc, "lastModified", DateTime.Now.ToString())),
                                    IsPublished = bool.Parse(ReadValue(doc, "ispublished", "true")),
                                };

                list.Add(post);
            }

            if (list.Count > 0)
            {
                list.Sort((p1, p2) => p2.PubDate.CompareTo(p1.PubDate));
            }

            return list;
        }

        private static string ReadValue(XElement doc, XName name, string defaultValue = "")
        {
            if (doc.Element(name) != null)
            {
                return doc.Element(name).Value;
            }

            return defaultValue;
        }
    }
}


