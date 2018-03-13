using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using VehicleTracker.StorageService.Context;
using Serilog;
using System.Threading;

namespace VehicleTracker.StorageService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                while (!SeedDatabase(host, services))
                {
                    Thread.Sleep(10000);
                }
            }

            host.Run();
        }

        public static bool SeedDatabase(IWebHost host, IServiceProvider services)
        {
            try
            {
                var context = services.GetRequiredService<VehicleContext>();
                DbInitializer.Initialize(context);
                return true;
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database. Retry after 10 sec");
                return false;
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services.AddAutofac())
                .UseSerilog((hostingContext, loggerConfiguration) =>
                    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration))
                .UseStartup<Startup>()
                .Build();
    }
}
