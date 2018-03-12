using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.Connector
{
    public class CachingVehicleService : IVehicleService
    {
        private readonly IVehicleService _underlying;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;

        public CachingVehicleService(IVehicleService underlying, IMemoryCache cache, ILogger<CachingVehicleService> logger)
        {
            _underlying = underlying;
            _cache = cache;
            _logger = logger;
        }

        public Task<VehicleStatusMessage> Ping(VehicleTrackRequest req)
        {
            var secondsAgo = (DateTime.UtcNow - req.TimeUtc).TotalSeconds;
            if (secondsAgo < 60)
            {
                return Ping(req.Id);
            }
            _logger.LogInformation($"Skipping outdated ping request for vehicle {req.Id}");
            return NullResult();
        }

        public Task<VehicleStatusMessage> Ping(string id)
        {
            if (!_cache.TryGetValue($"request:{id}", out bool value))
            {
                _cache.Set($"request:{id}", true, DateTimeOffset.UtcNow.AddSeconds(5));
                return _underlying.Ping(id);
            }
            else
            {
                _logger.LogInformation($"Skipping repeated ping request for vehicle {id}");
                return NullResult();
            }
        }

        private static Task<VehicleStatusMessage> NullResult() => Task.FromResult<VehicleStatusMessage>(null);
    }
}
