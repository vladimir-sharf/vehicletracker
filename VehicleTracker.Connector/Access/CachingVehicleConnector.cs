using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.Connector
{
    public class CachingVehicleConnector : IVehicleConnector
    {
        private readonly IVehicleConnector _underlying;
        private readonly IMemoryCache _cache;

        public CachingVehicleConnector(IVehicleConnector underlying, IMemoryCache cache)
        {
            _underlying = underlying;
            _cache = cache;
        }

        public Task<VehicleStatusMessage> Ping(string id)
        {
            return _cache.GetOrCreateAsync($"vehicle:{id}", entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromSeconds(60));
                return _underlying.Ping(id);
            });
        }
    }
}
