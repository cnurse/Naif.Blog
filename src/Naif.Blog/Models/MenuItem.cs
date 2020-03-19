namespace Naif.Blog.Models
{
    public class MenuItem
    {
        public string Action { get; set; }
        
        public string Controller { get; set; }
        
        public bool IsActive { get; set; }
        
        public string Link { get; set; }

        public string Text { get; set; }
    }
}