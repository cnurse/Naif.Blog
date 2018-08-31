using System;
using System.ComponentModel.DataAnnotations;
using Naif.Blog.XmlRpc;

namespace Naif.Blog.Models
{
    public class Page : PostBase
    {
        public Page()
        {
            PageId = Guid.NewGuid().ToString();
            ParentPageId = "0";
        }
        
        [XmlRpcProperty("page_id")]
        public string PageId { get; set; }
        
        [Display(Name="Page Template")]
        public string PageTemplate { get; set; }

        [Display(Name="Page Type")]
        public PageType PageType { get; set; }

        [XmlRpcProperty("wp_page_parent_id")]
        [Display(Name="Parent Page")]
        public string ParentPageId { get; set; }
    }
}