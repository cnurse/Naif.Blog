using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Naif.Blog.Framework;
using Naif.Blog.Services;
using Microsoft.AspNetCore.Builder;
using System.IO;
using Naif.Blog.Routing;
using Naif.Blog.Security;

namespace Naif.Blog
{
	// ReSharper disable once ClassNeverInstantiated.Global
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

	    // ReSharper disable once MemberCanBePrivate.Global
	    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
	    public IConfigurationRoot Configuration { get; set; }

	    // ReSharper disable once UnusedMember.Global
	    public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddTransient<IBlogRepository, JsonBlogRepository>();
            services.AddScoped<IApplicationContext, ApplicationContext>();
	        
	        services.Configure<XmlRpcSecurityOptions>(Configuration.GetSection("XmlRpcSecurity"));
	        
	        Auth0Config.ConfigureServices(services, Configuration);

            services.AddMvc();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ThemeViewLocationExpander());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	    // ReSharper disable once UnusedMember.Global
	    // ReSharper disable once UnusedParameter.Global
	    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
	        
	        app.UseAuthentication();

            app.UseApplicationContext();
	        
	        app.UseStatusCodePagesWithReExecute("/Error/Code/{0}");

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

            var config = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile("hosting.json", optional: true)
                .Build();

			var host = new WebHostBuilder()
				.UseKestrel()
                .UseConfiguration(config)
                .UseContentRoot(contentRoot)
                .UseWebRoot(Path.Combine(contentRoot, "wwwroot"))
                .UseIISIntegration()
				.UseStartup<Startup>()
				.Build();

			host.Run();
		}
    }
}