using System;
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
using VehicleTracker.Api.RealTime;

namespace VehicleTracker.Api.Configuration
{
    public static class ContainerConfiguration
    {
        public static ContainerBuilder AddVehicleTrackerServices(this ContainerBuilder builder)
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

            return builder;
        }
    }
}
