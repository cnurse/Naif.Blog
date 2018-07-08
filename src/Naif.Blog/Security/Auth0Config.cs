using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Naif.Blog.Security
{
    public static class Auth0Config
    {
        public static void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            // Add authentication services
			services.AddAuthentication(options => {
				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
			.AddCookie()
			.AddOpenIdConnect("Auth0", options => {
				// Set the authority to your Auth0 domain
				options.Authority = $"https://{configuration["Auth0:Domain"]}";
		
				// Configure the Auth0 Client ID and Client Secret
				options.ClientId = configuration["Auth0:ClientId"];
				options.ClientSecret = configuration["Auth0:ClientSecret"];
		
				// Set response type to code
				options.ResponseType = "code";
		
				// Configure the scope
				options.Scope.Clear();
				options.Scope.Add("openid");
				options.Scope.Add("profile");
				options.Scope.Add("email");
					
				// Set the callback path, so Auth0 will call back to http://localhost:5000/signin-auth0 
				// Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard 
				options.CallbackPath = new PathString("/signin-auth0");
		
				// Configure the Claims Issuer to be Auth0
				options.ClaimsIssuer = "Auth0";
					
				// Set the correct name claim type
				options.TokenValidationParameters = new TokenValidationParameters
				{
					NameClaimType = "name",
					RoleClaimType = "https://schemas.naifblog.com/roles"
				};
		
				options.Events = new OpenIdConnectEvents
				{
					// handle the logout redirection 
					OnRedirectToIdentityProviderForSignOut = (context) =>
					{
						var logoutUri = $"https://{configuration["Auth0:Domain"]}/v2/logout?client_id={configuration["Auth0:ClientId"]}";
		
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
        }
    }
}