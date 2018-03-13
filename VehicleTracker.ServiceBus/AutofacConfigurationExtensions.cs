using Autofac;

namespace VehicleTracker.ServiceBus 
{
    public static class AutofacConfigurationExtensions 
    {
        public static ContainerBuilder RegisterRabbitMq(this ContainerBuilder builder) 
        {
            builder.RegisterType<RabbitMqBus>()
                .As<IServiceBus>()
                .SingleInstance();

            builder.RegisterType<RabbitMqListenerFactory>()
                .As<IServiceBusListenerFactory>()
                .SingleInstance();

            return builder;
        }
    }
}