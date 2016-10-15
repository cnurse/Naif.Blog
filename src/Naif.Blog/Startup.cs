using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Naif.Blog.Framework;
using Naif.Blog.Models;
using Naif.Blog.Services;
using Microsoft.AspNetCore.Builder;
using System.IO;
using Naif.Blog.Routing;

namespace Naif.Blog
{
	public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration providers.
            var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddTransient<IBlogRepository, JsonBlogRepository>();
            services.AddScoped<IApplicationContext, ApplicationContext>();

            services.AddMvc();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ThemeViewLocationExpander());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseApplicationContext();

            app.UseMvc(routes =>
            {
				routes.MapRoute(
						name: "blogPost",
						template: "post/{*slug}",
						defaults: new {
							controller = "Blog",
							action = "ViewPost"
						})
					.MapRoute(
						name: "blogCategory",
						template: "category/{*category}",
						defaults: new
						{
							controller = "Blog",
							action = "ViewCategory"
						})
					.MapRoute(
						name: "blogTag",
						template: "tag/{*tag}",
						defaults: new
						{
							controller = "Blog",
							action = "ViewTag"
						})
					.MapRoute(
						name: "default",
						template: "{controller=Blog}/{action=Index}/{id?}")
                    .MapMetaWeblogRoute();
            });
        }

		// Entry point for the application.
		public static void Main(string[] args)
		{
            var contentRoot = Directory.GetCurrentDirectory();

			var host = new WebHostBuilder()
				.UseKestrel()
                .UseContentRoot(contentRoot) // Kestrel doesn't set ContentRoot by default
                .UseWebRoot(Path.Combine(contentRoot, "wwwroot")) // Kestrel doesn't set WebRoot by default
				//.UseIISIntegration()
				.UseStartup<Startup>()
				.Build();

			host.Run();
		}
    }
}