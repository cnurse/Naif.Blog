using Naif.Blog.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Newtonsoft.Json;
// ReSharper disable ClassNeverInstantiated.Global

namespace Naif.Blog.Services
{
    public class JsonPostRepository : FilePostRepository
    {
        public JsonPostRepository(IWebHostEnvironment env, IMemoryCache memoryCache, ILoggerFactory loggerFactory) : base(env, memoryCache)
        {
            Logger = loggerFactory.CreateLogger<JsonPostRepository>();
        }

        protected override string FileExtension => "json";

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
