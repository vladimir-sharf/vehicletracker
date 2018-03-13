using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracker.Api.Storage.StatusCache;
using VehicleTracker.ServiceBus.Messages;
using VehicleData = VehicleTracker.Api.Storage.Rest.Model.Vehicle;

namespace VehicleTracker.Api.Services
{
    public static class StatusCacheExtensions
    {
        public static async Task<Dictionary<string, VehicleStatusMessage>> ExtractStatuses(this IStatusCache cache, IEnumerable<VehicleData> fromStorage)
        {
            var statuses = new Dictionary<string, VehicleStatusMessage>();
            foreach (var vehicle in fromStorage)
            {
                var vehicleInfo = await cache.Get(vehicle.Id);
                statuses.Add(vehicle.Id, vehicleInfo);
            }
            return statuses;
        }

    }
}
