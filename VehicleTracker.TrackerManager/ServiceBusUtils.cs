using System.Threading.Tasks;
using VehicleTracker.ServiceBus;
using VehicleTracker.ServiceBus.Messages;
using VehicleTracker.TrackerManager.Services;

namespace VehicleTracker.TrackerManager
{
    public static class SeviceBusUtils
    {
        public static Task SubscribeEvents(this IServiceBusListenerFactory serviceBus, VehicleService vehicleService)
            => serviceBus.Create().SubscribeVehicleTrackSubscribe(t => vehicleService.Add(t.Ids));
    }
}
