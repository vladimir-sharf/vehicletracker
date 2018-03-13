using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.TrackerService
{
    public class VehicleFakeCache 
    {
        private readonly ConcurrentDictionary<string, VehicleFake> _cache;
        private readonly Func<VehicleFake> _VehicleFakeFactory;

        public VehicleFakeCache(Func<VehicleFake> VehicleFakeFactory) 
        {
            _cache = new ConcurrentDictionary<string, VehicleFake>();
            _VehicleFakeFactory = VehicleFakeFactory;
        }

        public async Task<VehicleStatus> GetStatus(string id) 
        {
            var model = _cache.GetOrAdd(id, i => _VehicleFakeFactory());
            var status = await model.Next();
            return status;
        }
    }
}