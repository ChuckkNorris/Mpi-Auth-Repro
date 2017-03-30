using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Http;
using Mpi.Auth.WebClient.Controllers;

namespace Mpi.Auth.WebClient
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           
            services.AddMvc();
            services.AddAuthorization(options => {
                options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("employeeNumber"));
            });
           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();
            app.UseBrowserLink();
           
            app.UseStaticFiles();
            app.UseCookieAuthentication(new CookieAuthenticationOptions { AuthenticationScheme = "Cookies" });
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions {
                AuthenticationScheme = "oidc",
                AutomaticChallenge = true,
                SignInScheme = "Cookies",
                Authority = "http://localhost:5000",
                // "https://mpiauthserver.azurewebsites.net", 
                RequireHttpsMetadata = false,

                ClientId = "mvc",
                ClientSecret = "secret",

                ResponseType = "code id_token",
                Scope = { "api1", "offline_access", "employee", "email" },

                GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true
            });
            // NOTE: Must use Authentication.Cookies @ 1.0.2 and Authentication.OpenIdConnect @ 1.0.2
            // https://github.com/IdentityServer/IdentityServer4/issues/505

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=index}/{id?}");
            });
        }
    }
}
