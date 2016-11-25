using System;
using System.Collections.Generic;
using Naif.Blog.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;

namespace Naif.Blog.Services
{
    public abstract class FileBlogRepository : IBlogRepository
    {
        readonly string _filesFolder;

        protected FileBlogRepository(IHostingEnvironment env, IMemoryCache memoryCache)
        {
            MemoryCache = memoryCache;
            PostsCacheKey = "{0}_posts";
            PostsFolder = Path.Combine("{0}", "posts", "{1}");
            _filesFolder = "/posts/{1}/files/";
            RootFolder = env.WebRootPath;
        }

        public abstract string FileExtension { get; }

        protected ILogger Logger {get; set;}

        protected IMemoryCache MemoryCache { get; set; }

        protected string PostsCacheKey { get; }

        protected string PostsFolder { get; }

        protected string RootFolder { get; }

        public virtual void Delete(Post post)
        {
            var cacheKey = string.Format(PostsCacheKey, post.BlogId);
            var postsFolder = string.Format(PostsFolder, RootFolder, post.BlogId);

            string file = Path.Combine(postsFolder, post.PostId + "." + FileExtension);

            File.Delete(file);

            MemoryCache.Remove(cacheKey);
            Logger.LogInformation($"{cacheKey} cleared.");
        }

        public IEnumerable<Post> GetAll(string blogId)
        {
            var cacheKey = String.Format(PostsCacheKey, blogId);

            IList<Post> posts;

            if (!MemoryCache.TryGetValue(cacheKey, out posts))
            {
                // fetch the value from the source
                posts = GetPosts(blogId).ToList();

                // store in the cache
                MemoryCache.Set(cacheKey,
                    posts,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(2)));
                Logger.LogInformation($"{cacheKey} updated from source.");
            }
            else
            {
                Logger.LogInformation($"{cacheKey} retrieved from cache.");
            }

            return posts;
        }

        public Dictionary<string, int> GetCategories(string blogId)
        {
            var result = GetAll(blogId).Where(p => ((p.IsPublished && p.PubDate <= DateTime.UtcNow)))
                .SelectMany(post => post.Categories)
                .GroupBy(category => category, (category, items) => new { Category = category, Count = items.Count() })
                .OrderBy(x => x.Category)
                .ToDictionary(x => x.Category, x => x.Count);

            return result;
        }

        protected abstract Post GetPost(string file, string blogId);

        private IEnumerable<Post> GetPosts(string blogId)
        {
            var postsFolder = String.Format(PostsFolder, RootFolder, blogId);

            if (!Directory.Exists(postsFolder))
            {
                Directory.CreateDirectory(postsFolder);
            }

            List<Post> list = new List<Post>();

            // Can this be done in parallel to speed it up?
            foreach (string file in Directory.EnumerateFiles(postsFolder, "*." + FileExtension, SearchOption.TopDirectoryOnly))
            {
                list.Add(GetPost(file, blogId));
            }

            if (list.Count > 0)
            {
                list.Sort((p1, p2) => p2.PubDate.CompareTo(p1.PubDate));
            }

            return list;
        }

        public Dictionary<string, int> GetTags(string blogId)
        {
            var result = GetAll(blogId).Where(p => ((p.IsPublished && p.PubDate <= DateTime.UtcNow)))
                .SelectMany(post => post.Tags)
                .GroupBy(tag => tag, (tag, items) => new { Tag = tag, Count = items.Count() })
                .OrderBy(x => x.Tag)
                .ToDictionary(x => x.Tag, x => x.Count);

            return result;
        }

        public void Save(Post post)
        {
            var cacheKey = string.Format(PostsCacheKey, post.BlogId);
            var postsFolder = string.Format(PostsFolder, RootFolder, post.BlogId);

            string file = Path.Combine(postsFolder, post.PostId + "." + FileExtension);
            post.LastModified = DateTime.UtcNow;

            SavePost(post, file);

            MemoryCache.Remove(cacheKey);

            Logger.LogInformation(!File.Exists(file)
                ? $"New Post - {post.PostId} created."
                : $"Post - {post.PostId} updated.");

            Logger.LogInformation($"{cacheKey} cleared.");
        }

        protected abstract void SavePost(Post post, string file);

        public string SaveMedia(string blogid, MediaObject media)
        {
            var filesFolder = String.Format(_filesFolder, blogid);

            if (!Directory.Exists(filesFolder))
            {
                Directory.CreateDirectory(filesFolder);
            }

            string extension = Path.GetExtension(media.Name);

            string relative = filesFolder + Guid.NewGuid();

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".bin";
            }
            else
            {
                extension = "." + extension.Trim('.');
            }

            relative += extension;

            string file = RootFolder + relative.Replace("/", "\\");

            File.WriteAllBytes(file, media.Bits);

            return relative;
        }
    }
}
