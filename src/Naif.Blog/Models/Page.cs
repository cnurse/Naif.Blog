using System;
using Naif.Blog.XmlRpc;

namespace Naif.Blog.Models
{
    public class Page
    {
        public Page()
        {
            PageId = Guid.NewGuid().ToString();
            ParentPageId = "0";
            BlogId = String.Empty;
            PubDate = DateTime.UtcNow;
            LastModified = DateTime.UtcNow;
            IsPublished = true;
            Keywords = String.Empty;
            Title = String.Empty;
            Slug = String.Empty;
            Content = String.Empty;
        }

        [XmlRpcProperty("blogId")]
        public string BlogId { get; set; }

        [XmlRpcProperty("description")]
        public string Content { get; set; }

        public bool IsPublished { get; set; }
        
        [XmlRpcProperty("mt_keywords")]
        public string Keywords { get; set; }

        [XmlRpcProperty("dateModified")]
        public DateTime LastModified { get; set; }
        
        [XmlRpcProperty("page_id")]
        public string PageId { get; set; }

        [XmlRpcProperty("wp_page_parent_id")]
        public string ParentPageId { get; set; }

        [XmlRpcProperty("dateCreated")]
        public DateTime PubDate { get; set; }

        [XmlRpcProperty("wp_slug")]
        public string Slug { get; set; }

        [XmlRpcProperty("title")]
        public string Title { get; set; }
    }
}