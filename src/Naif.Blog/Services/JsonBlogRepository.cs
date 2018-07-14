using System.Collections.Generic;
using Naif.Blog.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Newtonsoft.Json;

namespace Naif.Blog.Services
{
    public class JsonBlogRepository : FileBlogRepository
    {

        public JsonBlogRepository(IHostingEnvironment env,
                                    IMemoryCache memoryCache,
                                    ILoggerFactory loggerFactory)
            : base(env, memoryCache)
        {
            Logger = loggerFactory.CreateLogger<JsonBlogRepository>();
        }

        protected override string FileExtension { get { return "json"; } }

        protected override Page GetPage(string file, string blogId)
        {
            Page page;
            using (StreamReader r = File.OpenText(file))
            {
                string json = r.ReadToEnd();
                page =  JsonConvert.DeserializeObject<Page>(json);
                page.BlogId = blogId;
            }
            return page;
        }

        protected override Post GetPost(string file, string blogId)
        {
            Post post;
            using (StreamReader r = File.OpenText(file))
            {
                string json = r.ReadToEnd();
                post =  JsonConvert.DeserializeObject<Post>(json);
                post.BlogId = blogId;
            }
            return post;
        }

        protected override void SavePage(Page page, string file)
        {
            using (StreamWriter w = File.CreateText(file))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(w, page);
            }
        }

        protected override void SavePost(Post post, string file)
        {
            using (StreamWriter w = File.CreateText(file))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(w, post);
            }
        }
    }
}
