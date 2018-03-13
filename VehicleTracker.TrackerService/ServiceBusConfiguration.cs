using System.Threading.Tasks;
using VehicleTracker.ServiceBus;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.TrackerService
{
    public static class ServiceBusConfiguration
    {
        public static Task SubscribeEvents(this IServiceBusListenerFactory listener, IVehicleService vehicleService)
            => listener.Create().SubscribeVehicleTrackRequest(vehicleService.Ping);
    }
}
