using System;
using System.Collections.Generic;
using Naif.Blog.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Naif.Blog.Framework;
using Newtonsoft.Json;
// ReSharper disable ClassNeverInstantiated.Global

namespace Naif.Blog.Services
{
    public class FileBlogRepository : FileRepositoryBase, IBlogRepository
    {
        private string _blogsCacheKey = "blogs";
        private readonly string _blogsFile;
        private readonly IPostRepository _postRepository;
        private readonly string _filesFolder;
        private readonly string _fileUrl;
        private string _templatesCacheKey = "templates";
        private readonly string _templatesFolder;
        private string _themesCacheKey = "themes";
        private readonly string _themesFolder;
        private readonly RazorViewEngineOptions _viewEngineOptions;
        
        public FileBlogRepository(IHostingEnvironment env, 
                                    IOptions<RazorViewEngineOptions> optionsAccessor,
                                    IMemoryCache memoryCache, 
                                    ILoggerFactory loggerFactory, 
                                    IPostRepository postRepository) :base(env, memoryCache)
        {
            Logger = loggerFactory.CreateLogger<FileBlogRepository>();
            _postRepository = postRepository;
            _filesFolder = "{0}/posts/{1}/files/";
            _fileUrl = "/posts/{0}/files/{1}";
            _blogsFile = env.WebRootPath + @"\blogs.json";
            _themesFolder = env.WebRootPath + @"\themes\";
            _viewEngineOptions = optionsAccessor.Value;
        }

        protected override string FileExtension { get; }

        public IEnumerable<Models.Blog> GetBlogs()
        {
            return MemoryCache.GetObject(_blogsCacheKey, 
                Logger,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(2)),
                () =>
                {
                    IList<Models.Blog> blogs;
                    
                    // fetch the value from the source
                    using (StreamReader reader = File.OpenText(_blogsFile))
                    {
                        var json = reader.ReadToEnd();
                        blogs = JsonConvert.DeserializeObject<IList<Models.Blog>>(json);
                    }

                    return blogs;
                });
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

        public IEnumerable<string> GetTemplates(string blogId)
        {
            return MemoryCache.GetObject(_templatesCacheKey, 
                Logger,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(2)),
                () =>
                {
                    IList<string> templates = new List<string>();

                    var blog = GetBlogs().SingleOrDefault(b => b.Id == blogId);

                    if (blog != null)
                    {

                        foreach (var fileProvider in _viewEngineOptions.FileProviders)
                        {
                            var files = fileProvider.GetDirectoryContents($"/Views/Themes/{blog.Theme}/Templates");
                            if (files.Exists)
                            {
                                foreach (var file in files)
                                {
                                    templates.Add(file.Name.Replace(".cshtml", ""));
                                }
                            }
                        }
                    }

                    return templates;
                });
        }

        public IEnumerable<string> GetThemes()
        {
            return MemoryCache.GetObject(_themesCacheKey, 
                Logger,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(2)),
                () =>
                {
                    return Directory.GetDirectories(_themesFolder).Select(directory => directory.Replace(_themesFolder, string.Empty)).ToList();
                });
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
        
        public string SaveMedia(string blogId, MediaObject media)
        {
            var filesFolder = GetFolder(_filesFolder, blogId);

            if (!Directory.Exists(filesFolder))
            {
                Directory.CreateDirectory(filesFolder);
            }

            string extension = Path.GetExtension(media.Name);

            string fileName = Guid.NewGuid().ToString();

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".bin";
            }
            else
            {
                extension = "." + extension.Trim('.');
            }

            fileName += extension;

            string file = Path.Combine(filesFolder, fileName);

            File.WriteAllBytes(file, media.Bits);

            return String.Format(_fileUrl, blogId, fileName);
        }

    }
}
