using Naif.Blog.XmlRpc;
using System;
using System.Collections.Generic;

namespace Naif.Blog.Models
{
    public class Post
    {
        public Post()
        {
            ID = Guid.NewGuid().ToString();
            PubDate = DateTime.UtcNow;
            LastModified = DateTime.UtcNow;
            IsPublished = true;
        }

        [XmlRpcProperty("postid")]
        public string ID { get; set; }

        [XmlRpcProperty("title")]
        public string Title { get; set; }

        [XmlRpcProperty("author")]
        public string Author { get; set; }

        [XmlRpcProperty("wp_slug")]
        public string Slug { get; set; }

        [XmlRpcProperty("mt_excerpt")]
        public string Excerpt { get; set; }

        [XmlRpcProperty("description")]
        public string Content { get; set; }

        [XmlRpcProperty("dateCreated")]
        public DateTime PubDate { get; set; }

        [XmlRpcProperty("dateModified")]
        public DateTime LastModified { get; set; }

        [XmlRpcProperty("categories")]
        public string[] Categories { get; set; }

        [XmlRpcProperty("mt_keywords")]
        public string Keywords { get; set; }

        public bool IsPublished { get; set; }

    }
}
