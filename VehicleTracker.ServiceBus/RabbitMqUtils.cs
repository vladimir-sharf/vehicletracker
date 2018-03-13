using RabbitMQ.Client;

namespace VehicleTracker.ServiceBus
{
    public static class RabbitMqUtils 
    {
        public static IConnection CreateConnection(this RabbitMqOptions options)
        {
            var factory = new ConnectionFactory()
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                AutomaticRecoveryEnabled = true
            };
            return factory.CreateConnection();
        }   
    }
}