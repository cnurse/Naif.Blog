using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Naif.Blog.TagHelpers
{
    [HtmlTargetElement("section", Attributes = NameAttributeName)]
    public class UserProfileTagHelper : TagHelper
    {
        private const string NameAttributeName = "nb-profile-name";
        private const string EmailAttributeName = "nb-profile-email";
        private const string ImageAttributeName = "nb-profile-image";

        [HtmlAttributeName(NameAttributeName)]
        public string Name { get; set; }

        [HtmlAttributeName(EmailAttributeName)]
        public string Email { get; set; }

        [HtmlAttributeName(ImageAttributeName)]
        public string Image { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string content = @"<div class='col-md-2'>";

            if (string.IsNullOrEmpty(Image))
            {
                content += $@"<img src='{Image}' alt='' class='img-rounded img-responsive' />";
            }
            content += @"</div>";

            content += $@"<div class='col-md-4'><h3>{Name}</h3>";
            content += $@"<p><i class='glyphicon glyphicon-envelope'></i>{Email}</p>";
            content += @"</div>";

            output.Content.AppendHtml(content);
        }
    }
}