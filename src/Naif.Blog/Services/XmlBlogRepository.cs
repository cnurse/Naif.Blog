using System;
using System.Collections.Generic;
using System.Globalization;
using Naif.Blog.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;

namespace Naif.Blog.Services
{
    public class XmlBlogRepository : FileBlogRepository
    {
        public XmlBlogRepository(IHostingEnvironment env,
                                    IMemoryCache memoryCache,
                                    ILoggerFactory loggerFactory)
            : base(env, memoryCache)
        {
            Logger = loggerFactory.CreateLogger<XmlBlogRepository>();
        }

        protected override string FileExtension => "xml";

        protected override Page GetPage(string file, string blogId)
        {
            XElement doc = XElement.Load(file);

            var page = new Page()
            {
                PageId = Path.GetFileNameWithoutExtension(file),
                ParentPageId = ReadValue(doc, "parentPageId"),
                BlogId = blogId,
                Title = ReadValue(doc, "title"),
                Content = ReadValue(doc, "content"),
                Keywords = ReadValue(doc, "tags"),
                Slug = ReadValue(doc, "slug").ToLowerInvariant(),
                PubDate = DateTime.Parse(ReadValue(doc, "pubDate")),
                LastModified = DateTime.Parse(ReadValue(doc, "lastModified", DateTime.Now.ToString(CultureInfo.InvariantCulture))),
                IsPublished = bool.Parse(ReadValue(doc, "ispublished", "true")),
            };
            return page;
        }

        protected override Post GetPost(string file, string blogId)
        {
            XElement doc = XElement.Load(file);

            var post = new Post()
            {
                PostId = Path.GetFileNameWithoutExtension(file),
                BlogId = blogId,
                Title = ReadValue(doc, "title"),
                Author = ReadValue(doc, "author"),
                Excerpt = ReadValue(doc, "excerpt"),
                Content = ReadValue(doc, "content"),
                Keywords = ReadValue(doc, "tags"),
                Slug = ReadValue(doc, "slug").ToLowerInvariant(),
                PubDate = DateTime.Parse(ReadValue(doc, "pubDate")),
                LastModified = DateTime.Parse(ReadValue(doc, "lastModified", DateTime.Now.ToString(CultureInfo.InvariantCulture))),
                IsPublished = bool.Parse(ReadValue(doc, "ispublished", "true")),
            };

            LoadCategories(post, doc);
            return post;
        }

        protected override void SavePage(Page page, string file)
        {
            XDocument doc = new XDocument(
                new XElement("post",
                    new XElement("title", page.Title),
                    new XElement("slug", page.Slug),
                    new XElement("pubDate", page.PubDate.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("lastModified", page.LastModified.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("content", page.Content),
                    new XElement("categories", string.Empty),
                    new XElement("tags", page.Keywords),
                    new XElement("ispublished", page.IsPublished)
                ));

            using(var stream = new FileStream(file, FileMode.Create))
            {
                doc.Save(stream);
            }
        }

        protected override void SavePost(Post post, string file)
        {
            XDocument doc = new XDocument(
                            new XElement("post",
                                new XElement("title", post.Title),
                                new XElement("slug", post.Slug),
                                new XElement("author", post.Author),
                                new XElement("pubDate", post.PubDate.ToString("yyyy-MM-dd HH:mm:ss")),
                                new XElement("lastModified", post.LastModified.ToString("yyyy-MM-dd HH:mm:ss")),
                                new XElement("excerpt", post.Excerpt),
                                new XElement("content", post.Content),
                                new XElement("categories", string.Empty),
                                new XElement("tags", post.Keywords),
                                new XElement("ispublished", post.IsPublished)
                            ));

            XElement categories = doc.Document.Element("post").Element("categories");
            foreach (string category in post.Categories)
            {
                categories.Add(new XElement("category", category));
            }
            using(var stream = new FileStream(file, FileMode.Create))
            {
                doc.Save(stream);
            }
        }

        private static void LoadCategories(Post post, XElement doc)
        {
            XElement categories = doc.Element("categories");
            if (categories == null)
                return;

            List<string> list = new List<string>();

            foreach (var node in categories.Elements("category"))
            {
                list.Add(node.Value);
            }

            post.Categories = list.ToArray();
        }

        private static string ReadValue(XElement doc, XName name, string defaultValue = "")
        {
            if (doc.Element(name) != null)
            {
                return doc.Element(name).Value;
            }

            return defaultValue;
        }
    }
}


