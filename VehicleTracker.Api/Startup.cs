using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Autofac;
using VehicleTracker.Api.Storage.Rest;
using VehicleTracker.Api.Storage;
using VehicleTracker.Api.Storage.Rest.Model;
using VehicleFilter = VehicleTracker.Api.Model.VehicleFilter;
using CustomerFilter = VehicleTracker.Api.Model.CustomerFilter;
using VehicleTracker.ServiceBus;
using VehicleTracker.Api.Storage.StatusCache;
using VehicleTracker.Api.Services;
using Autofac.Core;
using VehicleTracker.Api.Configuration;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using VehicleTracker.Api.Hubs;
using System.Security.Claims;

namespace VehicleTracker.Api
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
            services.AddMvc();
            services.AddOptions();
            services.Configure<ServicesUriConfiguration>(Configuration);
            services.Configure<RabbitMqOptions>(Configuration.GetSection("RabbitMq"));
            services.AddMemoryCache();

            var domain = Configuration.GetValue<string>("Authentification:Authority");

            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.NameIdentifier);
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Authority = domain;
                    options.Audience = "VehicleTracker.Api";

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };                    
                });

            services.AddSignalR();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<RestRepository<string, Vehicle, VehicleFilter>>()
                .As<IRepository<string, Vehicle, VehicleFilter>>()
                .WithParameter(new ResolvedParameter(
                    (pi, context) => pi.ParameterType == typeof(string) && pi.Name == "baseUri",
                    (pi, context) => {
                        var options = context.Resolve<IOptions<ServicesUriConfiguration>>().Value;
                        return options.VehicleService;
                    }
                ))
                .InstancePerLifetimeScope();

            builder.RegisterType<RestRepository<Guid, Customer, CustomerFilter>>()
                .As<IRepository<Guid, Customer, CustomerFilter>>()
                .WithParameter(new ResolvedParameter(
                    (pi, context) => pi.ParameterType == typeof(string) && pi.Name == "baseUri",
                    (pi, context) => {
                        var options = context.Resolve<IOptions<ServicesUriConfiguration>>().Value;
                        return options.CustomerService;
                    }
                ))
                .InstancePerLifetimeScope();

            builder.RegisterType<QueryStringVehicleFilterTransformer>()
                .As<IQueryStringFilterTransformer<VehicleFilter>>()
                .SingleInstance();

            builder.RegisterGeneric(typeof(QueryStringVehicleTransformerDefault<>))
                .As(typeof(IQueryStringFilterTransformer<>))
                .SingleInstance();

            builder.RegisterType<RabbitMqBus>()
                .As<IServiceBus>()
                .SingleInstance();

            builder.RegisterType<StatusCache>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<VehicleService>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<CustomerService>()
                .AsSelf()
                .InstancePerLifetimeScope();
                
            builder.RegisterType<VehicleSubscription>()
                .AsSelf()
                .SingleInstance();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, IServiceBus serviceBus, StatusCache statusCache, VehicleSubscription vehicleSubscription)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"client-build")),
                RequestPath = new PathString(@"/client-build")
            });

            app.UseSignalR(routes => routes.MapHub<VehicleHub>("/vehicleHub"));
            app.UseMvc();
            lifetime.ApplicationStarted.Register(() => serviceBus.SubscribeEvents(statusCache, vehicleSubscription));
        }
    }
}
