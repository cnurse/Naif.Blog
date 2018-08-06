using System;
using System.Collections.Generic;
using Naif.Blog.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
// ReSharper disable ClassNeverInstantiated.Global

namespace Naif.Blog.Services
{
    public class FileBlogRepository : FileRepositoryBase, IBlogRepository
    {
        private string _blogsCacheKey = "blogs";
        private readonly string _blogsFile;
        private readonly IPostRepository _postRepository;
        
        public FileBlogRepository(IHostingEnvironment env, IMemoryCache memoryCache, ILoggerFactory loggerFactory, IPostRepository postRepository) :base(env, memoryCache)
        {
            Logger = loggerFactory.CreateLogger<FileBlogRepository>();
            _postRepository = postRepository;
            _blogsFile = env.WebRootPath + "/blogs.json";
        }

        protected override string FileExtension { get; }

        public IEnumerable<Models.Blog> GetBlogs()
        {
            IList<Models.Blog> blogs;

            if (!MemoryCache.TryGetValue(_blogsCacheKey, out blogs))
            {
                // fetch the value from the source
                using (StreamReader reader = File.OpenText(_blogsFile))
                {
                    var json = reader.ReadToEnd();
                    blogs = JsonConvert.DeserializeObject<IList<Models.Blog>>(json);
                }

                // store in the cache
                MemoryCache.Set(_blogsCacheKey,
                    blogs,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(2)));
                Logger.LogInformation($"{_blogsCacheKey} updated from source.");
            }
            else
            {
                Logger.LogInformation($"{_blogsCacheKey} retrieved from cache.");
            }

            return blogs;
        }

        public Dictionary<string, int> GetCategories(string blogId)
        {
            var result = _postRepository.GetAllPosts(blogId).Where(p => ((p.IsPublished && p.PubDate <= DateTime.UtcNow)))
                .SelectMany(post => post.Categories)
                .GroupBy(category => category, (category, items) => new { Category = category, Count = items.Count() })
                .OrderBy(x => x.Category)
                .ToDictionary(x => x.Category, x => x.Count);

            return result;
        }

        public Dictionary<string, int> GetTags(string blogId)
        {
            var result = _postRepository.GetAllPosts(blogId).Where(p => ((p.IsPublished && p.PubDate <= DateTime.UtcNow)))
                .SelectMany(post => post.Tags)
                .GroupBy(tag => tag, (tag, items) => new { Tag = tag, Count = items.Count() })
                .OrderBy(x => x.Tag)
                .ToDictionary(x => x.Tag, x => x.Count);

            return result;
        }

        public void SaveBlogs(IEnumerable<Models.Blog> blogs)
        {
            using (StreamWriter w = File.CreateText(_blogsFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(w, blogs);
            }
            
            MemoryCache.Remove(_blogsCacheKey);

            Logger.LogInformation($"Blog Settings updated.");

            Logger.LogInformation($"{_blogsCacheKey} cleared.");
        }
        
        public string SaveMedia(string blogid, MediaObject media)
        {
            var filesFolder = String.Format(FilesFolder, blogid);

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
