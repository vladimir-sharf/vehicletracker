using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using VehicleTracker.ServiceBus;
using VehicleTracker.Api.Storage.StatusCache;
using VehicleTracker.Api.Configuration;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using VehicleTracker.Api.RealTime;
using VehicleTracker.Api.Auth;

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

            services.AddVehicleTrackerAuth(Configuration);

            services.AddSignalR();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.AddVehicleTrackerServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, IServiceBusListenerFactory listenerFactory, IStatusCache statusCache, VehicleSubscription vehicleSubscription)
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
            lifetime.ApplicationStarted.Register(() => listenerFactory.SubscribeEvents(statusCache, vehicleSubscription));
        }
    }
}
