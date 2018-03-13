using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.Api.Storage.StatusCache
{
    public class StatusCache 
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<StatusCache> _logger;

        public StatusCache(IMemoryCache cache, ILogger<StatusCache> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<VehicleStatusMessage> Get(string id)
        {
            if (_cache.TryGetValue<VehicleStatusMessage>(id, out var value))
            {
                _logger.LogInformation($"Status for vehicle {id} in cache={value.Status}, updated={value.TimeUtc}");
                return Task.FromResult(value);
            }
            else
            {
                _logger.LogWarning($"Status for vehicle {id} not in cache");
                return Task.FromResult(new VehicleStatusMessage(id, VehicleStatus.Unknown));
            }
        }

        public Task<VehicleStatusMessage> Update(string id, VehicleStatusMessage vehicle)
        {
            _logger.LogTrace($"Cache update for vehicle {id}: {vehicle.Status}");
            var entry = _cache.Set(id, vehicle, DateTimeOffset.UtcNow.AddMinutes(2));
            return Task.FromResult(vehicle);
        }
    }
}
