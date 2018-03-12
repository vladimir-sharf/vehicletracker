using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.Connector
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleConnector _connector;
        private readonly IServiceBus _serviceBus;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;

        public VehicleService(IVehicleConnector connector, IServiceBus serviceBus, ILogger<VehicleService> logger, IMemoryCache cache)
        {
            _connector = connector;
            _serviceBus = serviceBus;
            _logger = logger;
        }

        public Task<VehicleStatusMessage> Ping(VehicleTrackRequest req)
            => Ping(req.Id);

        public async Task<VehicleStatusMessage> Ping(string id)
        {
            var result = await _connector.Ping(id);
            await _serviceBus.SendVehicleStatus(result);
            _logger.LogInformation($"Sent {id}: {result.Status}");
            return result;
        }
    }
}
