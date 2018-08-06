using Naif.Blog.Models;
using System.Collections.Generic;

namespace Naif.Blog.Services
{
    public interface IPostRepository
    {
        void DeletePost(Post post);

        IEnumerable<Post> GetAllPosts(string blogId);

        void SavePost(Post post);
    }
}
