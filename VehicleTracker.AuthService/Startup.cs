using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4;
using Microsoft.Extensions.Logging;

namespace VehicleTracker.AuthService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            var jsClientOptions = new JsClientOptions();
            Configuration.GetSection("JsClient").Bind(jsClientOptions);
            var domain = jsClientOptions.Domain;

            services.AddIdentityServer(options =>
            {
                options.IssuerUri = "http://vehicletracker.authservice";
            })
                .AddInMemoryClients(new List<Client>() {
                    new Client
                    {
                        ClientId = "js",
                        ClientName = "JavaScript Client",
                        AllowedGrantTypes = GrantTypes.Implicit,
                        AllowAccessTokensViaBrowser = true,

                        RedirectUris =           { $"{domain}/callback" },
                        PostLogoutRedirectUris = { domain },
                        AllowedCorsOrigins =     { domain },

                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "VehicleTracker.Api"
                        }
                    }                    
                })
                .AddInMemoryIdentityResources(new List<IdentityResource>() {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile()                    
                })
                .AddInMemoryApiResources(new List<ApiResource>() {
                    new ApiResource("VehicleTracker.Api", "Vehicle Tracker public api")                    
                })
                .AddTestUsers(new List<TestUser>(){
                    new TestUser() {
                        SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                        Username = "test",
                        Password = "test"
                    }})
                .AddDeveloperSigningCredential();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
