using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Naif.Blog.Framework;
using Naif.Blog.Services;
using Microsoft.AspNetCore.Builder;
using System.IO;
using Microsoft.Extensions.Hosting;
using Naif.Blog.Security;

namespace Naif.Blog
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class Startup
    {
        public Startup(IWebHostEnvironment env)
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
	        services.AddTransient<IBlogRepository, FileBlogRepository>();
            services.AddTransient<IPageRepository, JsonPageRepository>();
	        services.AddTransient<IPostRepository, JsonPostRepository>();
            services.AddScoped<IApplicationContext, ApplicationContext>();
	        
	        services.Configure<XmlRpcSecurityOptions>(Configuration.GetSection("XmlRpcSecurity"));
	        
	        Auth0Config.ConfigureServices(services, Configuration);

            services.AddMvc()
	            .AddRazorRuntimeCompilation();

            services.Configure<RazorViewEngineOptions>(options =>
            {
               options.ViewLocationExpanders.Add(new ThemeViewLocationExpander());
            });
	        
	        services.AddAuthorization(options =>
	        {
		        options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin"));
	        });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	    // ReSharper disable once UnusedMember.Global
	    // ReSharper disable once UnusedParameter.Global
	    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
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

	        app.UseRouting();

	        app.UseMetaWeblog();

	        app.UseAuthorization();

            app.UseEndpoints(endpoints  =>
            {
	            endpoints.MapControllers();
            });
        }
    }
}