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
using System.Net.Http;

namespace VehicleTracker.Api.Configuration
{
    public static class ContainerConfiguration
    {
        private static ResolvedParameter RepositoryParameter(Func<ServicesUriConfiguration, string> optionExtractor)
            => new ResolvedParameter(
                    (pi, context) => pi.ParameterType == typeof(HttpClient),
                    (pi, context) =>
                    {
                        var options = context.Resolve<IOptions<ServicesUriConfiguration>>().Value;
                        var baseUri = optionExtractor(options);
                        var client = context.Resolve<HttpClient>();
                        client.BaseAddress = new Uri(baseUri);
                        return client;
                    }
                );

        public static ContainerBuilder AddVehicleTrackerServices(this ContainerBuilder builder)
        {
            builder.RegisterType<HttpClient>()
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterType<RestRepository<string, Vehicle, VehicleFilter>>()
                .As<IRepository<string, Vehicle, VehicleFilter>>()
                .WithParameter(RepositoryParameter(o => o.VehicleService))
                .InstancePerLifetimeScope();

            builder.RegisterType<RestRepository<Guid, Customer, CustomerFilter>>()
                .As<IRepository<Guid, Customer, CustomerFilter>>()
                .WithParameter(RepositoryParameter(o => o.CustomerService))
                .InstancePerLifetimeScope();

            builder.RegisterType<QueryStringVehicleFilterTransformer>()
                .As<IQueryStringFilterTransformer<VehicleFilter>>()
                .SingleInstance();

            builder.RegisterGeneric(typeof(QueryStringFilterTransformerDefault<>))
                .As(typeof(IQueryStringFilterTransformer<>))
                .SingleInstance();

            builder.RegisterType<RabbitMqBus>()
                .As<IServiceBus>()
                .SingleInstance();

            builder.RegisterType<StatusCache>()
                .As<IStatusCache>()
                .SingleInstance();

            builder.RegisterType<VehicleService>()
                .As<IVehicleService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CustomerService>()
                .As<ICustomerService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<VehicleSubscription>()
                .AsSelf()
                .SingleInstance();

            return builder;
        }
    }
}
