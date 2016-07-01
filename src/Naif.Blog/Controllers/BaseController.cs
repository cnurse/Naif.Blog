using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Naif.Blog.Controllers
{
    public class BaseController : Controller
    {

        protected BaseController(IBlogRepository blogRepository)
        {
            BlogRepository = blogRepository;
        }

        protected IBlogRepository BlogRepository { get; set; }


        public string CreateSlug(string title)
        {
            title = title.ToLowerInvariant().Replace(" ", "-");
            title = RemoveDiacritics(title);
            title = RemoveReservedUrlCharacters(title);

            if (BlogRepository.GetAll().Any(p => string.Equals(p.Slug, title, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("Slug is already in use");
            }

            return title.ToLowerInvariant();
        }

        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private string RemoveReservedUrlCharacters(string text)
        {
            var reservedCharacters = new List<string>() { "!", "#", "$", "&", "'", "(", ")", "*", ",", "/", ":", ";", "=", "?", "@", "[", "]", "\"", "%", ".", "<", ">", "\\", "^", "_", "'", "{", "}", "|", "~", "`", "+" };

            foreach (var chr in reservedCharacters)
            {
                text = text.Replace(chr, "");
            }

            return text;
        }
    }
}
