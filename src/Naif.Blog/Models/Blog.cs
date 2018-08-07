using System.ComponentModel.DataAnnotations;

namespace Naif.Blog.Models
{
    public class Blog
    {
        [Display(Name = "By-Line")]
        public string ByLine { get; set; }

        public string Disclaimer { get; set; }

        public string GoogleAnalytics { get; set; }

        public string Id { get; set; }

        [Display(Name = "Local Url")]
        public string LocalUrl { get; set; }

        public string Theme { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }
    }
}


