using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Naif.Blog.Framework;
using Naif.Blog.Services;
using Microsoft.AspNetCore.Builder;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Naif.Blog.Routing;

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
	        
	        // Add authentication services
			services.AddAuthentication(options => {
				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
			.AddCookie()
			.AddOpenIdConnect("Auth0", options => {
				// Set the authority to your Auth0 domain
				options.Authority = $"https://{Configuration["Auth0:Domain"]}";
		
				// Configure the Auth0 Client ID and Client Secret
				options.ClientId = Configuration["Auth0:ClientId"];
				options.ClientSecret = Configuration["Auth0:ClientSecret"];
		
				// Set response type to code
				options.ResponseType = "code";
		
				// Configure the scope
				options.Scope.Clear();
				options.Scope.Add("openid");
		
				// Set the callback path, so Auth0 will call back to http://localhost:5000/signin-auth0 
				// Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard 
				options.CallbackPath = new PathString("/signin-auth0");
		
				// Configure the Claims Issuer to be Auth0
				options.ClaimsIssuer = "Auth0";
		
				options.Events = new OpenIdConnectEvents
				{
					// handle the logout redirection 
					OnRedirectToIdentityProviderForSignOut = (context) =>
					{
						var logoutUri = $"https://{Configuration["Auth0:Domain"]}/v2/logout?client_id={Configuration["Auth0:ClientId"]}";
		
						var postLogoutUri = context.Properties.RedirectUri;
						if (!string.IsNullOrEmpty(postLogoutUri))
						{
							if (postLogoutUri.StartsWith("/"))
							{
								// transform to absolute
								var request = context.Request;
								postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
							}
							logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
						}
		
						context.Response.Redirect(logoutUri);
						context.HandleResponse();
		
						return Task.CompletedTask;
					}
				};   
			});

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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
	        
	        app.UseAuthentication();

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