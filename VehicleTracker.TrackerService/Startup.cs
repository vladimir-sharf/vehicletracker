using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleTracker.ServiceBus;
using Autofac.Core;

namespace VehicleTracker.TrackerService
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
            services.AddMemoryCache();
            services.Configure<RabbitMqOptions>(Configuration.GetSection("RabbitMq"));
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<VehicleService>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<CachingVehicleService>()
                .AsSelf()
                .As<IVehicleService>()
                .SingleInstance()
                .WithParameter(new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(IVehicleService),
                        (pi, ctx) => ctx.Resolve<VehicleService>()
                    ));

            builder.RegisterType<VehicleConnectorFake>()
                .As<IVehicleConnector>()
                .SingleInstance();

            builder.RegisterType<RabbitMqBus>()
                .As<IServiceBus>()
                .SingleInstance();

            builder.RegisterType<VehicleFakeCache>()
                .AsSelf()
                .SingleInstance();

            builder.Register(ctx => new VehicleFake(0.5d, 0.2d, 0, 5))
                .AsSelf()
                .InstancePerDependency();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, IServiceBus serviceBus, IVehicleService vehicleService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            lifetime.ApplicationStarted.Register(() => serviceBus.SubscribeEvents(vehicleService));
        }
    }
}
