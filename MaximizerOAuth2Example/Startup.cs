using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MaximizerOAuth2Example
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // In this example we're just using simple cookie-based authentication
            services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "Maximizer";                
            })
            .AddCookie()
            .AddOAuth("Maximizer", options =>
            {
                // The ClientId and ClientSecret are generated when you create your Maximizer OAuth client
                // https://developer.maximizer.com/doc/maximizerwebauthentication/setting-oauth-client-profile-maximizer
                options.ClientId = Configuration["Maximizer:ClientId"];
                options.ClientSecret = Configuration["Maximizer:ClientSecret"];
                // Your authorization and token endpoints are determined by the URL of your Maximizer installation
                // https://developer.maximizer.com/doc/maximizerwebauthentication/maximizerwebauthentication-endpoints
                options.AuthorizationEndpoint = Configuration["Maximizer:AuthorizationEndpoint"];
                options.TokenEndpoint = Configuration["Maximizer:TokenEndpoint"];
                // This is the path to which your app will be redirected after authenticating; here we're just using the homepage
                options.CallbackPath = new Microsoft.AspNetCore.Http.PathString("/login-maximizer");
                // Because we want to use the tokens to make API calls, we need to save them
                options.SaveTokens = true;
            });

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
