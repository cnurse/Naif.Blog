﻿using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Services;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Naif.Blog.Controllers
{
    public abstract class BaseController : Controller
    {
        protected BaseController(IBlogRepository blogRepository, IApplicationContext appContext)
        {
            Blog = appContext.CurrentBlog;
            BlogRepository = blogRepository;
        }

        protected Models.Blog Blog { get; }

        protected IBlogRepository BlogRepository { get; set; }

        protected string CreateSlug(string title)
        {
            title = title.ToLowerInvariant().Replace(" ", "-");
            title = RemoveDiacritics(title);
            title = RemoveReservedUrlCharacters(title);

//            if (BlogRepository.GetAllPosts(Blog.Id).Any(p => string.Equals(p.Slug, title, StringComparison.OrdinalIgnoreCase)))
//            {
//                throw new Exception("Slug is already in use");
//            }

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
