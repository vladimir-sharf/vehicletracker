using Microsoft.Extensions.Configuration;
using System;
using VehicleTracker.ServiceBus;

namespace VehicleTracker.UnitTests
{
    public static class ConfigurationUtils
    {
        private static Lazy<IConfiguration> _configuration = new Lazy<IConfiguration>(ReadConfiguration);
        public static IConfiguration Configuration => _configuration.Value;

        private static Lazy<RabbitMqOptions> _rabbitMqOptions = new Lazy<RabbitMqOptions>(GetRabbitMqOptions);
        public static RabbitMqOptions RabbitMqOptions => _rabbitMqOptions.Value;

        private static IConfiguration ReadConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            return configuration;
        }

        private static RabbitMqOptions GetRabbitMqOptions()
        {
            var options = new RabbitMqOptions();
            Configuration.GetSection("RabbitMq").Bind(options);
            return options;
        }
    }
}
