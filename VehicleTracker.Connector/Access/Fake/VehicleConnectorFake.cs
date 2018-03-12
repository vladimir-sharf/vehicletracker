using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.Connector
{
    public class VehicleConnectorFake : IVehicleConnector
    {
        private VehicleFakeCache _vehicleFakeCache;
        private ILogger _logger;

        public VehicleConnectorFake(VehicleFakeCache vehicleFakeCache, ILogger<VehicleConnectorFake> logger)
        {
            _vehicleFakeCache = vehicleFakeCache;
            _logger = logger;
        }

        public async Task<VehicleStatusMessage> Ping(string id) 
        {
            var status = await _vehicleFakeCache.GetStatus(id);
            _logger.LogInformation($"Vehicle {id} replied: {status}");
            return new VehicleStatusMessage(id, status);
        }
    }
}
