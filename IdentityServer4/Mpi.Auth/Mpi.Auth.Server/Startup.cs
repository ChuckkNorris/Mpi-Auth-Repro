using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using IdentityServer4;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using Microsoft.DotNet.PlatformAbstractions;
using System.Diagnostics;
//using System.IdentityModel.Tokens;
//using Microsoft.Extensions.PlatformAbstractions;

namespace Mpi.Auth.Server
{
    public class Startup
    {
        // You don't need any DB connection string - currently configured for In-Memory store
        private const string CONNECTION_STRING = "CONNECTIONSTRING FOR Config & Operational Store here";
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var certificatePath = $@"{ApplicationEnvironment.ApplicationBasePath}\certs\idsrv3test.pfx";
            Console.WriteLine($"Certificate Path: {certificatePath}");
            Debug.WriteLine($"Cert Path: {certificatePath}");
            var cert = new X509Certificate2(certificatePath, "idsrv3test");
            //var clause = new keyiden() , new SecurityKeyIdentifier(new SecurityKeyIdentifierClause)
            //var som = new Microsoft.IdentityModel.Tokens.X509SecurityKey(cert);
            //var secKey = new Microsoft.IdentityModel.Tokens.SigningCredentials(som, "");
         //   var creds = new X509SigningCredentials(cert);
            services.AddIdentityServer()
             //   .AddTemporarySigningCredential()
                .AddTestUsers(Config.GetUsers())
                .AddSigningCredential(cert)
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients());
                //.AddConfigurationStore(b => b.UseSqlServer(CONNECTION_STRING, options => options.MigrationsAssembly(migrationsAssembly)))
                //.AddOperationalStore(b => b.UseSqlServer(CONNECTION_STRING, options => options.MigrationsAssembly(migrationsAssembly)));
                
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //InitializeDatabase(app);
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Sitefinity RelyingParty encryption Key
            app.UseIdentityServer();
            app.UseGoogleAuthentication(new GoogleOptions {
                AuthenticationScheme = "Google",
                DisplayName = "Google",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,

                ClientId = "1234", 
                ClientSecret = "1234"
            });
            app.UseLinkedInAuthentication(new AspNet.Security.OAuth.LinkedIn.LinkedInAuthenticationOptions {
                AuthenticationScheme = "LinkedIn",
                DisplayName = "LinkedIn",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                ClientId = "1234",
                ClientSecret = "1234"
            });
            app.UseFacebookAuthentication(new FacebookOptions {
                AuthenticationScheme = "Facebook",
                DisplayName = "Facebook",
                AppId = "",
                AppSecret = "",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                ClientId = "1234",
                ClientSecret = "1234"
            });
            app.UseStaticFiles();
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=index}/{id?}");
            });
            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            //app.UseMvcWithDefaultRoute();

        }

        private void InitializeDatabase(IApplicationBuilder app) {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope()) {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any()) {
                    foreach (var client in Config.GetClients()) {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any()) {
                    foreach (var resource in Config.GetIdentityResources()) {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any()) {
                    foreach (var resource in Config.GetApiResources()) {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
