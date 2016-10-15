using Naif.Blog.Models;
using System.Collections.Generic;

namespace Naif.Blog.Services
{
    public interface IBlogRepository
    {
        void Delete(Post post);

        IEnumerable<Post> GetAll(string blogId);

        Dictionary<string, int> GetCategories(string blogId);

        Dictionary<string, int> GetTags(string blogId);

        void Save(Post post);

        string SaveMedia(string blogid, MediaObject media);
    }
}
