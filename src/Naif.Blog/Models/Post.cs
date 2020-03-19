using Naif.Blog.XmlRpc;
using System;

namespace Naif.Blog.Models
{
    public class Post : PostBase
    {
        public Post()
        {
            PostId = Guid.NewGuid().ToString();
            Author = String.Empty;
        }

        [XmlRpcProperty("author")]
        public string Author { get; set; }

        [XmlRpcProperty("postid")]
        public string PostId { get; set; }

    }
}
