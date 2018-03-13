﻿using System.Threading.Tasks;
using VehicleTracker.ServiceBus;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.TrackerService
{
    public static class ServiceBusConfiguration
    {
        public static IServiceBus SubscribeEvents(this IServiceBus serviceBus, IVehicleService vehicleService)
            => serviceBus.SubscribeVehicleTrackRequest(vehicleService.Ping);
    }
}