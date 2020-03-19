using System.Collections.Generic;
using Naif.Blog.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using System.Linq;
// ReSharper disable ConvertClosureToMethodGroup

namespace Naif.Blog.Services
{
    public abstract class FilePostRepository : FileRepositoryBase, IPostRepository
    {
        private readonly string _postsCacheKey;
        private readonly string _postsFolder;

        protected FilePostRepository(IWebHostEnvironment env, IMemoryCache memoryCache) : base(env, memoryCache)
        {
            _postsCacheKey = "{0}_posts";
            _postsFolder = Path.Combine("{0}", "posts", "{1}");
        }

        public virtual void DeletePost(Post post)
        {
            DeleteObject(post, post.PostId, _postsCacheKey, _postsFolder);
        }

        public IEnumerable<Post> GetAllPosts(string blogId)
        {
            return GetObjects(_postsCacheKey, _postsFolder, blogId, (f, id) => GetPost(f, id)).ToList();
        }

        protected abstract Post GetPost(string file, string blogId);


        public void SavePost(Post post)
        {
            SaveObject(post, post.PostId, _postsCacheKey, _postsFolder,  (p, f) => SavePost(p, f));
        }

        protected abstract void SavePost(Post post, string file);
    }
}
