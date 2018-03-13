using System.Threading.Tasks;
using VehicleTracker.Api.RealTime;
using VehicleTracker.Api.Storage.StatusCache;
using VehicleTracker.ServiceBus;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.Api
{
    public static class ServiceBusUtils
    {
        public static IServiceBus SubscribeEvents(this IServiceBus serviceBus, StatusCache statusCache, VehicleSubscription vehicleSubscription)
            => serviceBus
                .SubscribeVehicleInfo(t => statusCache.Update(t.Id, t))
                .SubscribeVehicleInfo(vehicleSubscription.Send);
    }
}
