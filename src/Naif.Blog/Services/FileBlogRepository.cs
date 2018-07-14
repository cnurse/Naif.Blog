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
        private readonly string _filesFolder;
        private readonly IMemoryCache _memoryCache;
        private readonly string _pagesCacheKey;
        private readonly string _pagesFolder;
        private readonly string _postsCacheKey;
        private readonly string _postsFolder;
        private readonly string _rootFolder;

        protected FileBlogRepository(IHostingEnvironment env, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _pagesCacheKey = "{0}_pages";
            _pagesFolder = Path.Combine("{0}", "pages", "{1}");
            _postsCacheKey = "{0}_posts";
            _postsFolder = Path.Combine("{0}", "posts", "{1}");
            _filesFolder = "/posts/{0}/files/";
            _rootFolder = env.WebRootPath;
        }

        protected abstract string FileExtension { get; }

        protected ILogger Logger {get; set;}

        public virtual void DeletePost(Post post)
        {
            var cacheKey = string.Format(_postsCacheKey, post.BlogId);
            var postsFolder = string.Format(_postsFolder, _rootFolder, post.BlogId);

            string file = Path.Combine(postsFolder, post.PostId + "." + FileExtension);

            File.Delete(file);

            _memoryCache.Remove(cacheKey);
            Logger.LogInformation($"{cacheKey} cleared.");
        }

        public IEnumerable<Page> GetAllPages(string blogId)
        {
            var cacheKey = String.Format(_pagesCacheKey, blogId);

            IList<Page> pages;

            if (!_memoryCache.TryGetValue(cacheKey, out pages))
            {
                // fetch the value from the source
                pages = GetPages(blogId).ToList();

                // store in the cache
                _memoryCache.Set(cacheKey,
                    pages,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(2)));
                Logger.LogInformation($"{cacheKey} updated from source.");
            }
            else
            {
                Logger.LogInformation($"{cacheKey} retrieved from cache.");
            }

            return pages;
        }

        public IEnumerable<Post> GetAllPosts(string blogId)
        {
            var cacheKey = String.Format(_postsCacheKey, blogId);

            IList<Post> posts;

            if (!_memoryCache.TryGetValue(cacheKey, out posts))
            {
                // fetch the value from the source
                posts = GetPosts(blogId).ToList();

                // store in the cache
                _memoryCache.Set(cacheKey,
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
            var result = GetAllPosts(blogId).Where(p => ((p.IsPublished && p.PubDate <= DateTime.UtcNow)))
                .SelectMany(post => post.Categories)
                .GroupBy(category => category, (category, items) => new { Category = category, Count = items.Count() })
                .OrderBy(x => x.Category)
                .ToDictionary(x => x.Category, x => x.Count);

            return result;
        }

        protected abstract Page GetPage(string file, string blogId);

        private IEnumerable<Page> GetPages(string blogId)
        {
            var pagesFolder = String.Format(_pagesFolder, _rootFolder, blogId);

            if (!Directory.Exists(pagesFolder))
            {
                Directory.CreateDirectory(pagesFolder);
            }

            List<Page> list = new List<Page>();

            // Can this be done in parallel to speed it up?
            foreach (string file in Directory.EnumerateFiles(pagesFolder, "*." + FileExtension, SearchOption.TopDirectoryOnly))
            {
                list.Add(GetPage(file, blogId));
            }

            if (list.Count > 0)
            {
                list = list.OrderByDescending(p => p.PubDate).ToList();
            }

            return list;
        }

        protected abstract Post GetPost(string file, string blogId);

        private IEnumerable<Post> GetPosts(string blogId)
        {
            var postsFolder = String.Format(_postsFolder, _rootFolder, blogId);

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
                list = list.OrderByDescending(p => p.PubDate).ToList();
            }

            return list;
        }

        public Dictionary<string, int> GetTags(string blogId)
        {
            var result = GetAllPosts(blogId).Where(p => ((p.IsPublished && p.PubDate <= DateTime.UtcNow)))
                .SelectMany(post => post.Tags)
                .GroupBy(tag => tag, (tag, items) => new { Tag = tag, Count = items.Count() })
                .OrderBy(x => x.Tag)
                .ToDictionary(x => x.Tag, x => x.Count);

            return result;
        }

        public void SavePage(Page page)
        {
            var cacheKey = string.Format(_pagesCacheKey, page.BlogId);
            var pagesFolder = string.Format(_pagesFolder, _rootFolder, page.BlogId);

            string file = Path.Combine(pagesFolder, page.PageId + "." + FileExtension);
            page.LastModified = DateTime.UtcNow;

            SavePage(page, file);

            _memoryCache.Remove(cacheKey);

            Logger.LogInformation(!File.Exists(file)
                ? $"New Page - {page.PageId} created."
                : $"Page - {page.PageId} updated.");

            Logger.LogInformation($"{cacheKey} cleared.");
        }

        public void SavePost(Post post)
        {
            var cacheKey = string.Format(_postsCacheKey, post.BlogId);
            var postsFolder = string.Format(_postsFolder, _rootFolder, post.BlogId);

            string file = Path.Combine(postsFolder, post.PostId + "." + FileExtension);
            post.LastModified = DateTime.UtcNow;

            SavePost(post, file);

            _memoryCache.Remove(cacheKey);

            Logger.LogInformation(!File.Exists(file)
                ? $"New Post - {post.PostId} created."
                : $"Post - {post.PostId} updated.");

            Logger.LogInformation($"{cacheKey} cleared.");
        }

        protected abstract void SavePage(Page page, string file);

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

            string file = _rootFolder + relative.Replace("/", "\\");

            File.WriteAllBytes(file, media.Bits);

            return relative;
        }
    }
}
