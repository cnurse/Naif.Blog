using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Naif.Blog.XmlRpc;
using Newtonsoft.Json;

namespace Naif.Blog.Models
{
    public abstract class PostBase
    {
        protected PostBase()
        {
            BlogId = String.Empty;
            Categories = new string[] { };
            Content = String.Empty;
            Excerpt = String.Empty;
            IsPublished = false;
            Keywords = String.Empty;
            LastModified = DateTime.UtcNow;
            PubDate = DateTime.UtcNow;
            Slug = String.Empty;
            Title = String.Empty;
        }
 
        [XmlRpcProperty("blogId")]
        public string BlogId { get; set; }

        [XmlRpcProperty("categories")]
        public string[] Categories { get; set; }

        [XmlRpcProperty("description")]
        public string Content { get; set; }
        
        [XmlRpcProperty("mt_excerpt")]
        public string Excerpt { get; set; }

        public bool IsPublished { get; set; }

        [XmlRpcProperty("mt_keywords")]
        public string Keywords { get; set; }

        [XmlRpcProperty("dateModified")]
        public DateTime LastModified { get; set; }

        [XmlRpcProperty("dateCreated")]
        public DateTime PubDate { get; set; }

        [XmlRpcProperty("wp_slug")]
        public string Slug { get; set; }
        
        [JsonIgnore]
        public List<string> Tags
        {
            get
            {
                List<string> tags = new List<string>();
                if (!string.IsNullOrEmpty(Keywords))
                {
                    tags.AddRange(Keywords.Split(',').Select(tag => tag.Trim()));
                }
                return tags;
            }
        }

        [Required]
        [XmlRpcProperty("title")]
        public string Title { get; set; }
    }
}