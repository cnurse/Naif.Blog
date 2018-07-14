using Naif.Blog.Models;
using System.Collections.Generic;

namespace Naif.Blog.Services
{
    public interface IBlogRepository
    {
        void DeletePost(Post post);

        IEnumerable<Page> GetAllPages(string blogId);

        IEnumerable<Post> GetAllPosts(string blogId);

        Dictionary<string, int> GetCategories(string blogId);

        Dictionary<string, int> GetTags(string blogId);

        void SavePage(Page page);
        
        void SavePost(Post post);

        string SaveMedia(string blogid, MediaObject media);
    }
}
