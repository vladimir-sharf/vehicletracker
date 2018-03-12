﻿using System.Threading.Tasks;
using VehicleTracker.ServiceBus;
using VehicleTracker.ServiceBus.Messages;
using VehicleTracker.TrackerService.Services;

namespace VehicleTracker.TrackerService
{
    public static class SeviceBusUtils
    {
        public static IServiceBus SubscribeEvents(this IServiceBus serviceBus, VehicleService vehicleService)
            => serviceBus.SubscribeVehicleTrackSubscribe(t => vehicleService.Add(t.Ids));
    }
}
