using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleTracker.ServiceBus;
using VehicleTracker.TrackerManager.Configuration;
using VehicleTracker.TrackerManager.Services;
using VehicleTracker.TrackerManager.Storage;

namespace VehicleTracker.TrackerManager
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
            services.AddSingleton<IServiceBus, RabbitMqBus>();
            services.AddSingleton<IServiceBusListenerFactory, RabbitMqListenerFactory>();
            services.AddSingleton<IVehicleRepository, VehicleRepositoryInMemory>();
            services.AddSingleton<VehicleService>();
            services.Configure<TrackingOptions>(Configuration);
            services.Configure<RabbitMqOptions>(Configuration.GetSection("RabbitMq"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, IServiceBusListenerFactory listener, VehicleService vehicleService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            lifetime.ApplicationStarted.Register(() =>
            {
                vehicleService.StartJob();
                listener.SubscribeEvents(vehicleService);
            });
        }
    }
}
