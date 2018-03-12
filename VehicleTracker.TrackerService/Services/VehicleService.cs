using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus;
using VehicleTracker.ServiceBus.Messages;
using VehicleTracker.TrackerService.Configuration;
using VehicleTracker.TrackerService.Model;
using VehicleTracker.TrackerService.Storage;

namespace VehicleTracker.TrackerService.Services
{
    public class VehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IServiceBus _serviceBus;
        private readonly TimeSpan _timeout;
        private readonly ILogger _logger;

        public VehicleService(IVehicleRepository vehicleRepository, IServiceBus serviceBus, IOptions<TrackingOptions> options, ILogger<VehicleService> logger)
        {
            _vehicleRepository = vehicleRepository;
            _serviceBus = serviceBus;
            _timeout = TimeSpan.FromSeconds(options.Value.TrackingTimeout);
            _logger = logger;
        }

        public void StartJob()
        {
            Observable
                .Timer(DateTimeOffset.UtcNow, _timeout)
                .Subscribe(next => QueryVehicles());
        }

        public async Task QueryVehicles()
        {
            _logger.LogInformation("Query vehicles task activated");
            var subs = await _vehicleRepository.Get();
            _logger.LogInformation($"Found {subs.Count()} vehicles to query");
            foreach (var s in subs)
                await _serviceBus.SendVehicleTrackRequest(new VehicleTrackRequest(s.Id));
            _logger.LogInformation("Query vehicles task complete.");
        }

        public Task<IEnumerable<VehicleSubscription>> Get() =>
            _vehicleRepository.Get();

        public async Task Add(IEnumerable<string> ids)
        {
            foreach (var id in ids)
                await Add(id);
        }

        public Task Add(string id)
        {
            _logger.LogInformation($"Adding vehicle {id} for query list");
            return _vehicleRepository.Add(new VehicleSubscription(id, DateTime.UtcNow));
        }

        public Task Remove(string id)
        {
            _logger.LogInformation($"Removing vehicle {id} from query list");
            return _vehicleRepository.Remove(id);
        }
    }
}
